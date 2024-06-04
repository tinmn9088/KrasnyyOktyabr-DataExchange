using System.Text;
using Microsoft.Extensions.Logging;

namespace KrasnyyOktyabr.ComV77Application.Logging;

public static partial class LoggingHelper
{
    public static string NullString => "null";

    public static string BuildArgsString(object?[]? args)
    {
        if (args is null)
        {
            return NullString;
        }

        StringBuilder result = new();

        for (int i = 0; i < args.Length; i++)
        {
            if (i > 0)
            {
                result.Append(' ');
            }

            result.Append('{');
            result.Append(args[i]?.ToString() ?? NullString);
            result.Append('}');
        }

        return result.ToString();
    }

    [LoggerMessage(EventId = 1001, Level = LogLevel.Trace, Message = "Trying connect to '{infobasePath}'")]
    public static partial void TryingConnectAsync(this ILogger logger, string infobasePath);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Trace, Message = "Invoke '{memberName}' with args: {args}")]
    public static partial void InvokingMember(this ILogger logger, string memberName, string args);

    [LoggerMessage(EventId = 1003, Level = LogLevel.Trace, Message = "Connection '{infobasePath}' was inactive for {disposeTimeout}")]
    public static partial void DisposeTimeoutExceeded(this ILogger logger, string infobasePath, TimeSpan disposeTimeout);

    [LoggerMessage(EventId = 1004, Level = LogLevel.Trace, Message = "Releasing COM object of connection '{infobasePath}'")]
    public static partial void ReleasingComObject(this ILogger logger, string infobasePath);

    [LoggerMessage(EventId = 1005, Level = LogLevel.Trace, Message = "Dispose connection '{infobasePath}' from factory")]
    public static partial void DisposingConnectionFromFactory(this ILogger logger, string infobasePath);
}
