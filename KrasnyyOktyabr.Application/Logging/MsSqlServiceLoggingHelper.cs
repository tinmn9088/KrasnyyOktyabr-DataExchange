namespace KrasnyyOktyabr.Application.Logging;

public static partial class MsSqlServiceLoggingHelper
{
    [LoggerMessage(EventId = 50001, Level = LogLevel.Trace, Message = "{insertedRowsCount} rows inserted")]
    public static partial void Inserted(this ILogger logger, int insertedRowsCount);
}
