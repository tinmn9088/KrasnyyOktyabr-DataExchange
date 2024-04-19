namespace KrasnyyOktyabr.Application.Logging;

public static partial class WmiServiceLoggingHelper
{
    [LoggerMessage(EventId = 1000001, Level = LogLevel.Trace, Message = "Reading property '{propertyName}' from WMI object '{wmiPath}'")]
    public static partial void ReadingProperty(this ILogger logger, string propertyName, string wmiPath);
}
