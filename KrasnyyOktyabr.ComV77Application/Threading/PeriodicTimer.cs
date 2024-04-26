namespace KrasnyyOktyabr.ComV77Application.Threading;

public class PeriodicTimer : IDisposable
{
    public PeriodicTimer(TimeSpan period)
    {
        _isDisposed = false;

        _disposeCancellationTokenSource = new();

        _innerTimer = new(ReleaseWaitLock);

        _waitLock = new(0, 1);

        Period = period;
    }

    public TimeSpan Period
    {
        get
        {
            return _period;
        }
        set
        {
            _period = value;

            if (!_innerTimer.Change(_period, _period))
            {
                throw new InvalidOperationException("Failed to update timer");
            }
        }
    }

    private TimerCallback ReleaseWaitLock => (_) =>
    {
        if (_isWaited)
        {
            _waitLock.Release();
        }
    };

    private TimeSpan _period;

    private bool _isDisposed;

    private bool _isWaited;

    private readonly CancellationTokenSource _disposeCancellationTokenSource;

    private readonly Timer _innerTimer;

    /// <summary>
    /// Most of the time it is closed. It is released only when <see cref="_innerTimer"/> elapses and
    /// must to be closed again immediately.
    /// </summary>
    private readonly SemaphoreSlim _waitLock;

    public async ValueTask<bool> WaitForNextTickAsync()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(ToString());
        }

        try
        {
            _isWaited = true;

            await _waitLock.WaitAsync(_disposeCancellationTokenSource.Token);

            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        finally
        {
            _isWaited = false;
        }
    }

    public void Dispose()
    {
        _isDisposed = true;

        _disposeCancellationTokenSource.Cancel();

        _innerTimer.Dispose();

        _disposeCancellationTokenSource.Dispose();
    }
}
