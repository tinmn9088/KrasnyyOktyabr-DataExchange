using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using KrasnyyOktyabr.ComV77Application.Contracts.Configuration;
using Microsoft.Extensions.Logging;
using static KrasnyyOktyabr.ComV77Application.Logging.LoggingHelper;

namespace KrasnyyOktyabr.ComV77Application;

[SupportedOSPlatform("windows")]
public sealed class ComV77ApplicationConnection : IComV77ApplicationConnection
{
    public static int MaxErrorsCount => 3;

    public static TimeSpan InitializeTimeout => TimeSpan.FromSeconds(30);

    public static TimeSpan DisposeTimeout => TimeSpan.FromSeconds(15);

    private static string ComObjectTypeName => "V77.Application";

    private readonly ILogger _logger;

    private readonly ConnectionProperties _properties;

    private readonly SemaphoreSlim _connectionLock;

    private Type? _comObjectType;

    private object? _comObject;

    private bool _isInitialized;

    private bool _isDisposed;

    private int _retrievedTimes;

    private int _errorsCount;

    private readonly Action _removeFromFactoryCallback;

    /// <remarks>
    /// When disposed <see cref="_actualDisposeTask"/> stops without releasing COM object.
    /// </remarks>
    private readonly PeriodicTimer _actualDisposeTimer;

    /// <summary>
    /// Releases COM object when <see cref="_actualDisposeTimer"/> is elapsed.
    /// </summary>
    private readonly Task _actualDisposeTask;

    private ComV77ApplicationConnection(ConnectionProperties properties, Action removeFromFactoryCallback, ILogger<ComV77ApplicationConnection> logger)
    {
        _logger = logger;
        _properties = properties;
        _connectionLock = new(1);

        _isInitialized = false;
        _isDisposed = false;

        _retrievedTimes = 0;
        _errorsCount = 0;

        _removeFromFactoryCallback = removeFromFactoryCallback;

        _actualDisposeTimer = new(Timeout.InfiniteTimeSpan); // Turned off timer

        _actualDisposeTask = Task.Run(async () =>
        {
            while (await _actualDisposeTimer.WaitForNextTickAsync().ConfigureAwait(false)) // Runs until timer is disposed
            {
                _logger.DisposeTimeoutExceeded(_properties.InfobasePath, DisposeTimeout);

                await _connectionLock.WaitAsync().ConfigureAwait(false);

                try
                {
                    if (_retrievedTimes == 0)
                    {
                        _isDisposed = true;

                        _removeFromFactoryCallback();

                        _actualDisposeTimer.Dispose();

                        ReleaseComObject();
                    }
                }
                finally
                {
                    _connectionLock.Release();
                }
            }
        });
    }

    /// <exception cref="FailedToCreateTypeException"></exception>
    /// <exception cref="FailedToCreateComObjectException"></exception>
    /// <exception cref="InitializeTimeoutExceededException"></exception>
    /// <exception cref="ErrorsCountExceededException"></exception>
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        _logger.TryingConnectAsync(_properties.InfobasePath);

        await _connectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            CheckCurrentState(isInitializing: true);

            _comObjectType ??= CreateType();

            _comObject ??= CreateComObject();

            if (!_isInitialized)
            {
                _isInitialized = await InitializeComObjectAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception)
        {
            _errorsCount++;
            throw;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    /// <param name="isInitializing">
    /// When <c>true</c> checks <see cref="_errorsCount"/> and <see cref="_isDisposed"/>,
    /// when <c>false</c> - <see cref="_comObject"/> and <see cref="_isInitialized"/> also.
    /// </param>
    /// <exception cref="ErrorsCountExceededException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    private void CheckCurrentState(bool isInitializing = false)
    {
        if (_errorsCount >= MaxErrorsCount)
        {
            throw new ErrorsCountExceededException();
        }

        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (!isInitializing)
        {
            if (_comObject == null)
            {
                throw new InvalidOperationException("COM object is not created");
            }

            if (!_isInitialized)
            {
                throw new InvalidOperationException("COM object is not connected to infobase");
            }
        }
    }

    /// <exception cref="FailedToCreateTypeException"></exception>
    private static Type? CreateType()
    {
        try
        {
            return Type.GetTypeFromProgID(ComObjectTypeName, true);
        }
        catch (Exception ex)
        {
            throw new FailedToCreateTypeException(ex);
        }
    }

    /// <remarks>
    /// Underlying <c>1cv7.exe</c> process starts.
    /// </remarks>
    /// <exception cref="FailedToCreateComObjectException"></exception>
    private object? CreateComObject()
    {
        try
        {
            return Activator.CreateInstance(_comObjectType!);
        }
        catch (Exception ex)
        {
            throw new FailedToCreateComObjectException(ex);
        }
    }

    /// <summary>
    /// Call <c>Initialize</c> method on <see cref="_comObject"/> and wait until
    /// method returns or <see cref="InitializeTimeout"/> is exceeded.
    /// </summary>
    /// <returns>
    /// Value returned by <c>Initialize</c> method.
    /// </returns>
    /// <exception cref="InitializeTimeoutExceededException"></exception>
    private async Task<bool> InitializeComObjectAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        object rmtrade = InvokeMember(
            memberName: "RMTrade",
            attributes: BindingFlags.GetProperty,
            binder: null,
            target: _comObject,
            args: null,
            isInitializing: true)!;

        // To prevent timeoutExceededTask from starting before initializeTask
        SemaphoreSlim initializeTaskStarted = new(0, 1);

        Task<bool> initializeTask = Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            initializeTaskStarted.Release();

            return (bool)InvokeMember(
                memberName: "Initialize",
                attributes: BindingFlags.InvokeMethod,
                binder: null,
                target: _comObject,
                args: [rmtrade!, $" /D{_properties.InfobasePath} /N{_properties.Username} /P{_properties.Password}", "NO_SPLASH_SHOW"],
                isInitializing: true)!;
        });

        Task timeoutExceededTask = Task.Run(async () =>
        {
            await initializeTaskStarted.WaitAsync(cancellationToken).ConfigureAwait(false);

            await Task.Delay(InitializeTimeout, cancellationToken);
        }, cancellationToken);

        bool timeoutExceeded = await Task.WhenAny(initializeTask, timeoutExceededTask).ConfigureAwait(false) != initializeTask;

        if (timeoutExceeded)
        {
            throw new InitializeTimeoutExceededException();
        }

        return initializeTask.Result;
    }

    /// <exception cref="OperationCanceledException"></exception>
    public async Task<object?> RunErtAsync(string ertName, Dictionary<string, string>? ertContext, string? resultName, CancellationToken cancellationToken = default)
    {
        await _connectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            CheckCurrentState(isInitializing: false);

            string ertFullPath = Path.Combine(_properties.InfobasePath, ertName);

            cancellationToken.ThrowIfCancellationRequested();

            // CreateObject("ValueList")
            object contextValueList = InvokeMember(
                memberName: "CreateObject",
                attributes: BindingFlags.Public | BindingFlags.InvokeMethod,
                binder: null,
                target: _comObject,
                args: ["ValueList"])!;

            if (ertContext != null)
            {
                foreach (KeyValuePair<string, string> nameValue in ertContext)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // ValueList.AddValue(Value, Name)
                    InvokeMember(
                        type: contextValueList.GetType(),
                        memberName: "AddValue",
                        attributes: BindingFlags.Public | BindingFlags.InvokeMethod,
                        binder: null,
                        target: contextValueList,
                        args: [nameValue.Value, nameValue.Key]);
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            // OpenForm(ObjectName, Context, FullPath)
            InvokeMember(
                memberName: "OpenForm",
                attributes: BindingFlags.Public | BindingFlags.InvokeMethod,
                binder: null,
                target: _comObject,
                args: ["Report", contextValueList, ertFullPath]);

            cancellationToken.ThrowIfCancellationRequested();

            // ValueList.Get(Name)
            object? result = InvokeMember(
                type: contextValueList.GetType(),
                memberName: "Get",
                attributes: BindingFlags.Public | BindingFlags.InvokeMethod,
                binder: null,
                target: contextValueList,
                args: [resultName]);

            return result;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    /// <summary>
    /// Calls <see cref="Type.InvokeMember(string, BindingFlags, Binder?, object?, object?[]?)"/> on <see cref="_comObjectType"/>.
    /// </summary>
    /// <remarks>
    /// Updates <see cref="_lastActivity"/>.
    /// </remarks>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="FailedToInvokeMemberException"></exception>
    /// <exception cref="ErrorsCountExceededException"></exception>
    private object? InvokeMember(string memberName, BindingFlags attributes, Binder? binder, object? target, object?[]? args, bool isInitializing = false)
    {
        CheckCurrentState(isInitializing: isInitializing);

        try
        {
            return InvokeMember(_comObjectType!, memberName, attributes, binder, target, args);
        }
        catch (Exception ex)
        {
            if (!isInitializing) // To prevent from incrementing errors count twice
            {
                _errorsCount++;
            }

            throw new FailedToInvokeMemberException(memberName, args, ex);
        }
    }

    /// <remarks>
    /// Updates <see cref="_lastActivity"/>.
    /// </remarks>
    private object? InvokeMember(Type type, string memberName, BindingFlags attributes, Binder? binder, object? target, object?[]? args)
    {
        _logger.InvokingMember(memberName, BuildArgsString(args));

        return type.InvokeMember(memberName, attributes, binder, target, args);
    }

    /// <summary>
    /// Sets period of <see cref="_actualDisposeTimer"/> to <see cref="DisposeTimeout"/>.
    /// </summary>
    private void TurnOnActualDisposeTimer()
    {
        _actualDisposeTimer.Period = DisposeTimeout;
    }

    /// <summary>
    /// Sets period of <see cref="_actualDisposeTimer"/> to <see cref="Timeout.InfiniteTimeSpan"/>.
    /// </summary>
    private void TurnOffActualDisposeTimer()
    {
        _actualDisposeTimer.Period = Timeout.InfiniteTimeSpan;
    }

    /// <summary>
    /// Turns on timer which will release resources after <see cref="DisposeTimeout"/> of inactivity.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await _connectionLock.WaitAsync().ConfigureAwait(false);

        try
        {
            if (_retrievedTimes >= 1)
            {
                _retrievedTimes--;
            }

            if (_retrievedTimes == 0)
            {
                if (!_isDisposed)
                {
                    TurnOnActualDisposeTimer();
                }
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    /// <remarks>
    /// <para>
    /// Underlying <c>1cv7.exe</c> process stops.
    /// </para>
    /// </remarks>
    private void ReleaseComObject()
    {
        _logger.ReleasingComObject(_properties.InfobasePath);

        if (_comObject != null) Marshal.FinalReleaseComObject(_comObject);
        _isInitialized = false;
        _comObject = null;
        _comObjectType = null;

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public sealed class Factory(ILogger<ComV77ApplicationConnection> logger) : IComV77ApplicationConnectionFactory
    {
        private readonly SemaphoreSlim _factoryLock = new(1);

        private readonly Dictionary<ConnectionProperties, ComV77ApplicationConnection> _propertiesConnections = [];

        private bool _isDisposed = false;

        /// <exception cref="ObjectDisposedException"></exception>
        public async Task<IComV77ApplicationConnection> GetConnectionAsync(ConnectionProperties connectionProperties, CancellationToken cancellationToken = default)
        {
            await _factoryLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                ObjectDisposedException.ThrowIf(_isDisposed, this);

                if (_propertiesConnections.TryGetValue(connectionProperties, out ComV77ApplicationConnection? connection))
                {
                    await connection._connectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);

                    try
                    {
                        if (!connection._isDisposed)
                        {
                            connection._retrievedTimes++;

                            connection.TurnOffActualDisposeTimer();

                            return connection;
                        }
                    }
                    finally
                    {
                        connection._connectionLock.Release();
                    }
                }

                ComV77ApplicationConnection newConnection = new(
                    connectionProperties,
                    removeFromFactoryCallback: () => _propertiesConnections.Remove(connectionProperties),
                    logger);

                _propertiesConnections.Add(connectionProperties, newConnection);

                return newConnection;
            }
            finally
            {
                _factoryLock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _factoryLock.WaitAsync().ConfigureAwait(false);

            try
            {
                _isDisposed = true;

                foreach (ComV77ApplicationConnection connection in _propertiesConnections.Values)
                {
                    logger.DisposingConnectionFromFactory(connection._properties.InfobasePath);

                    connection._isDisposed = true;

                    connection._actualDisposeTimer.Dispose();

                    await connection._actualDisposeTask;

                    connection.ReleaseComObject();
                }

                _propertiesConnections.Clear();
            }
            finally
            {
                _factoryLock.Release();
            }
        }
    }

    public class FailedToCreateTypeException : Exception
    {
        internal FailedToCreateTypeException(Exception innerException)
            : base($"Failed to create type '{ComObjectTypeName}'", innerException)
        {
        }
    }

    public class FailedToCreateComObjectException : Exception
    {
        internal FailedToCreateComObjectException(Exception innerException)
            : base($"Failed to object of type '{ComObjectTypeName}'", innerException)
        {
        }
    }

    public class InitializeTimeoutExceededException : Exception
    {
        internal InitializeTimeoutExceededException()
            : base($"Initialize timeout exceeded ({InitializeTimeout})")
        {
        }
    }

    public class ErrorsCountExceededException : Exception
    {
        internal ErrorsCountExceededException()
            : base($"Errors count exceeded ({MaxErrorsCount})")
        {
        }
    }

    public class FailedToInvokeMemberException : Exception
    {
        internal FailedToInvokeMemberException(string memberName, object?[]? args, Exception innerException)
            : base($"Failed to invoke member '{memberName}' with args: {BuildArgsString(args)}", innerException)
        {
        }
    }
}
