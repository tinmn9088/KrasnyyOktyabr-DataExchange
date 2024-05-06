using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace KrasnyyOktyabr.ApplicationNet48.Logging;

public static class KafkaLoggingHelper
{
    public static void LogStarting(this ILogger logger)
    {
        logger.LogTrace("Starting ...");
    }

    public static void LogStarted(this ILogger logger)
    {
        logger.LogTrace("Started");
    }

    public static void LogErrorOnStart(this ILogger logger, Exception exception)
    {
        logger.LogError(exception, "Error on start");
    }

    public static void LogStartingWatchingInfobaseChanges(this ILogger logger, string key)
    {
        logger.LogTrace("Starting watching infobase '{Key}' changes", key);
    }

    public static void LogProcessingInfobaseChanges(this ILogger logger, string key)
    {
        logger.LogTrace("Processing infobase '{Key}' changes", key);
    }

    public static void LogStartingPeriodProduceJob(this ILogger logger, string key, DateTimeOffset start, DateTimeOffset end)
    {
        logger.LogTrace("Starting period produce job on '{Key}' ({Start} - {End})", key, start, end);
    }

    public static void LogConfigurationNotFound(this ILogger logger)
    {
        logger.LogTrace("Configuration not found");
    }

    public static void LogConfigurationFound(this ILogger logger, int count)
    {
        logger.LogTrace("{Count} configurations found", count);
    }

    public static void LogConsumerGroupNotSpecified(this ILogger logger)
    {
        logger.LogTrace("Consumer group not specified");
    }

    public static void LogErrorsExceeded(this ILogger logger, string key)
    {
        logger.LogTrace("'{Key}' errors exceeded", key);
    }

    public static void LogRestarting(this ILogger logger)
    {
        logger.LogTrace("Restarting ...");
    }

    public static void LogRestarting(this ILogger logger, string key)
    {
        logger.LogTrace("Restarting '{Key}' ...", key);
    }

    public static void LogRestarted(this ILogger logger)
    {
        logger.LogTrace("Restarted");
    }

    public static void LogRestarted(this ILogger logger, string key)
    {
        logger.LogTrace("Restarted '{Key}'", key);
    }

    public static void LogOffsetInvalidFormat(this ILogger logger, string offset)
    {
        logger.LogTrace("Invalid offset format: '{Offset}'", offset);
    }

    public static void LogRequestInfobaseChanges(this ILogger logger, string key)
    {
        logger.LogTrace("Request infobase '{Key}' changes", key);
    }

    public static void LogGettingNewTransactions(this ILogger logger, string key)
    {
        logger.LogTrace("Getting new transactions from infobase '{Key}'", key);
    }

    public static void LogGettingTransactionsForPeriod(this ILogger logger, string key, DateTimeOffset start, DateTimeOffset end)
    {
        logger.LogTrace("Getting transactions from infobase '{Key}' for {Start} - {End}", key, start, end);
    }

    public static void LogGettingObjectsFromInfobase(this ILogger logger, string key)
    {
        logger.LogTrace("Getting objects from infobase '{Key}'", key);
    }

    public static void LogGotObjectsFromInfobase(this ILogger logger, int objectsCount, string key)
    {
        logger.LogTrace("Got {ObjectsCount} objects from infobase '{Key}'", objectsCount, key);
    }

    public static void LogSendingObjects(this ILogger logger, int objectsCount)
    {
        logger.LogTrace("Sending {objectsCount} objects", objectsCount);
    }

    public static void LogProducedMessage(this ILogger logger, string topic, string key, string message)
    {
        logger.LogTrace(
            "Message sent to '{Topic}' (key: '{Key}', length: {Length}): {ShortenedMessage}",
            topic,
            key,
            message.Length,
            ShortenMessage(message, 400));
    }

    public static void LogConsumedMessage(this ILogger logger, string consumerGroup, string topic, string key, int length, string message)
    {
        logger.LogTrace(
            "'{ConsumerGroup}' consuming message from '{Topic}' (key: '{Key}', length: {Length}): {ShortenedMessage}",
            consumerGroup,
            topic,
            key,
            length,
            ShortenMessage(message, 400));
    }

    public static void LogJsonTransformResult(this ILogger logger, int resultsCount)
    {
        logger.LogTrace("Interpreter result is {Count} object", resultsCount);
    }

    public static void LogJsonTransformError(this ILogger logger, Exception exception)
    {
        logger.LogError(exception, "Interpreter error");
    }

    public static void LogSavingObjects(this ILogger logger, int objectsCount)
    {
        logger.LogTrace("Saving {ObjectsCount} objects", objectsCount);
    }

    public static void LogPeriodProduceJobFinished(this ILogger logger, string key, DateTimeOffset start, DateTimeOffset end)
    {
        logger.LogTrace("Period produce job on '{Key}' ({Start} - {End}) finished", key, start, end);
    }

    public static void LogCancellingPeriodProduceJob(this ILogger logger, string key, DateTimeOffset start, DateTimeOffset end)
    {
        logger.LogTrace("Cancelling period produce job on '{Key}' ({Start} - {End})", key, start, end);
    }

    public static void LogCancelledPeriodProduceJob(this ILogger logger, string key, DateTimeOffset start, DateTimeOffset end)
    {
        logger.LogTrace("Cancelled period produce job on '{Key}' ({Start} - {End})", key, start, end);
    }

    public static void LogOperationCancelled(this ILogger logger)
    {
        logger.LogWarning("Operation cancelled");
    }

    public static void LogPeriodProduceJobError(this ILogger logger, Exception exception, string key, DateTimeOffset start, DateTimeOffset end)
    {
        logger.LogError(exception, "Period produce job '{Key}' ({Start} - {End}) error", key, start, end);
    }

    public static void LogProducerError(this ILogger logger, Exception exception)
    {
        logger.LogError(exception, "Producer error");
    }

    public static void LogConsumerError(this ILogger logger, Exception exception)
    {
        logger.LogError(exception, "Consumer error");
    }

    public static void LogStoppingProducers(this ILogger logger, int count)
    {
        logger.LogTrace("Stopping {Count} producers", count);
    }

    public static void LogStoppingConsumers(this ILogger logger, int count)
    {
        logger.LogTrace("Stopping {Count} consumers", count);
    }

    public static void LogStopping(this ILogger logger)
    {
        logger.LogTrace("Stopping ...");
    }

    public static void LogStopped(this ILogger logger)
    {
        logger.LogTrace("Stopped");
    }

    public static void LogDisposing(this ILogger logger)
    {
        logger.LogTrace("Disposing ...");
    }

    public static void LogDisposed(this ILogger logger)
    {
        logger.LogTrace("Disposed");
    }

    public static void LogDisposing(this ILogger logger, string key)
    {
        logger.LogTrace("Disposing '{Key}' ...", key);
    }

    public static void LogDisposed(this ILogger logger, string key)
    {
        logger.LogTrace("Disposed '{Key}'", key);
    }

    public static void LogDisposingPeriodProduceJob(this ILogger logger, string key, DateTimeOffset start, DateTimeOffset end)
    {
        logger.LogTrace("Disposing '{Key}' ({Start} - {End}) ...", key, start, end);
    }

    public static void LogDisposedPeriodProduceJob(this ILogger logger, string key, DateTimeOffset start, DateTimeOffset end)
    {
        logger.LogTrace("Disposed '{Key}' ({Start} - {End})", key, start, end);
    }

    public static void LogDisposedPeriodProduceJobs(this ILogger logger, int jobsCount)
    {
        logger.LogTrace("Disposed {JobsCount} jobs", jobsCount);
    }

    public static void LogAlreadyDisposed(this ILogger logger, string key)
    {
        logger.LogError("Already disposed '{Key}'", key);
    }

    public static string ShortenMessage(string message, int lengthLimit)
    {
        if (message.Length <= lengthLimit)
        {
            return message;
        }

        string startEndSeparator = " ... ";

        if (message.Length <= startEndSeparator.Length)
        {
            return message;
        }

        StringBuilder stringBuilder = new();

        int leftOffset = (lengthLimit - startEndSeparator.Length) / 2;
        int rightOffset = message.Length - (lengthLimit - (leftOffset + startEndSeparator.Length));

        stringBuilder.Append(message.Substring(0, leftOffset));
        stringBuilder.Append(startEndSeparator);
        stringBuilder.Append(message.Substring(rightOffset));

        return stringBuilder.ToString();
    }
}
