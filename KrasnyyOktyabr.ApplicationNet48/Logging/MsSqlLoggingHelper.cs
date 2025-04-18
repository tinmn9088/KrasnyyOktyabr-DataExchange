using Microsoft.Extensions.Logging;
using static KrasnyyOktyabr.ApplicationNet48.Services.IMsSqlService;

namespace KrasnyyOktyabr.ApplicationNet48.Logging;

public static class MsSqlLoggingHelper
{
    public static void LogConnecting(this ILogger logger, ConnectionType connectionType)
    {
        logger.LogTrace("Connection with '{ConnectionType}' connection type", connectionType);
    }
}
