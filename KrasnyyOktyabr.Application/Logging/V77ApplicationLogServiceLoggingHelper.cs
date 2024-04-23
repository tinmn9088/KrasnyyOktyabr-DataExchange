namespace KrasnyyOktyabr.Application.Logging;

public static partial class V77ApplicationLogServiceLoggingHelper
{
    [LoggerMessage(EventId = 20001, Level = LogLevel.Trace, Message = "{linesRead} lines read")]
    public static partial void LinesRead(this ILogger logger, int linesRead);

    [LoggerMessage(EventId = 20010, Level = LogLevel.Warning, Message = "No filter start position")]
    public static partial void NoFilterStartPosition(this ILogger logger);

    [LoggerMessage(EventId = 20011, Level = LogLevel.Trace, Message = "File length ({fileStreamLength}) is less than limit ({seekBackBytesLimits})")]
    public static partial void FileLengthIsLessThanSeekBackBytesLimit(this ILogger logger, long fileStreamLength, long seekBackBytesLimits);

    [LoggerMessage(EventId = 20012, Level = LogLevel.Trace, Message = "Start reading from position {position}")]
    public static partial void StartReading(this ILogger logger, long position);

    [LoggerMessage(EventId = 20013, Level = LogLevel.Trace, Message = "Found the last line before '{prefix}' for {iterationsCount} at {position}: '{line}'")]
    public static partial void SearchByPrefixResult(this ILogger logger, long position, string prefix, string line, int iterationsCount);

    [LoggerMessage(EventId = 2004, Level = LogLevel.Trace, Message = "Start reading period ({start} - {end}) from position {position}")]
    public static partial void StartReadingPeriod(this ILogger logger, long position, DateTimeOffset start, DateTimeOffset end);

    [LoggerMessage(EventId = 20023, Level = LogLevel.Trace, Message = "First line read: '{firstLineRead}'")]
    public static partial void FirstLineRead(this ILogger logger, string firstLineRead);

    [LoggerMessage(EventId = 20023, Level = LogLevel.Trace, Message = "First line read in period ({start} - {end}): '{firstLineRead}'")]
    public static partial void FirstLineReadInPeriod(this ILogger logger, string firstLineRead, DateTimeOffset start, DateTimeOffset end);

    [LoggerMessage(EventId = 20034, Level = LogLevel.Trace, Message = "Committed line found '{committedLine} ({transactionsCleared} found transactions cleared)'")]
    public static partial void CommittedLineFound(this ILogger logger, string committedLine, int transactionsCleared);

    [LoggerMessage(EventId = 20065, Level = LogLevel.Trace, Message = "{linesReadCound} lines read, last line: '{lastReadLine}' ({transactionsCount} transactions found)")]
    public static partial void LastReadLine(this ILogger logger, int linesReadCound, string lastReadLine, int transactionsCount);

    [LoggerMessage(EventId = 20065, Level = LogLevel.Trace, Message = "{linesReadCound} lines read in period ({start} - {end}), last line: '{lastReadLine}' ({transactionsCount} transactions found)")]
    public static partial void LastReadLineInPeriod(this ILogger logger, int linesReadCound, string lastReadLine, int transactionsCount, DateTimeOffset start, DateTimeOffset end);
}
