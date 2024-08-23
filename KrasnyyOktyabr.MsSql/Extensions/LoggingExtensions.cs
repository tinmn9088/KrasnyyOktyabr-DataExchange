using Microsoft.Extensions.Logging;

namespace MsSql.Extensions;

internal static class LoggingExtensions
{
    public static void LogConnecting(this ILogger logger, IMsSqlService.ConnectionType connectionType)
    {
        logger.LogTrace("Connection with '{ConnectionType}' connection type", connectionType);
    }
}
