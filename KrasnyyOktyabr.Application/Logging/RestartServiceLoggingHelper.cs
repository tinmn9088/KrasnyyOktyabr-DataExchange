namespace KrasnyyOktyabr.Application.Logging;

public static partial class RestartServiceLoggingHelper
{
    [LoggerMessage(EventId = 200000, Level = LogLevel.Trace, Message = "Starting ...")]
    public static partial void Starting(this ILogger logger);

    [LoggerMessage(EventId = 200001, Level = LogLevel.Trace, Message = "Restarting ...")]
    public static partial void Restarting(this ILogger logger);

    [LoggerMessage(EventId = 200003, Level = LogLevel.Trace, Message = "Restarted")]
    public static partial void Restarted(this ILogger logger);

    [LoggerMessage(EventId = 200023, Level = LogLevel.Trace, Message = "Checking status")]
    public static partial void CheckingStatus(this ILogger logger);

    [LoggerMessage(EventId = 200033, Level = LogLevel.Trace, Message = "Found inactive '{typeName}', key '{key}'")]
    public static partial void InactiveFound(this ILogger logger, string typeName, string key);

    [LoggerMessage(EventId = 200034, Level = LogLevel.Trace, Message = "Minimal timeout not expired yet for '{typeName}', key '{key}' (can be restarted after {restartAfter})")]
    public static partial void MinRestartTimeoutNotExpired(this ILogger logger, string typeName, string key, DateTimeOffset restartAfter);

    [LoggerMessage(EventId = 200200, Level = LogLevel.Trace, Message = "Stopping ...")]
    public static partial void Stopping(this ILogger logger);

    [LoggerMessage(EventId = 200200, Level = LogLevel.Trace, Message = "Stopped")]
    public static partial void Stopped(this ILogger logger);

    [LoggerMessage(EventId = 200201, Level = LogLevel.Trace, Message = "Operation cancelled")]
    public static partial void OperationCancelled(this ILogger logger);

    [LoggerMessage(EventId = 200219, Level = LogLevel.Trace, Message = "Disposing")]
    public static partial void Disposing(this ILogger logger);

    [LoggerMessage(EventId = 200220, Level = LogLevel.Trace, Message = "Disposed")]
    public static partial void Disposed(this ILogger logger);
}
