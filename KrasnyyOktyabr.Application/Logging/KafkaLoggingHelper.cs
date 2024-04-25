using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KrasnyyOktyabr.Application.Logging;

public static partial class KafkaLoggingHelper
{
    [LoggerMessage(EventId = 4001, Level = LogLevel.Trace, Message = "Configuration not found")]
    public static partial void ConfigurationNotFound(this ILogger logger);

    [LoggerMessage(EventId = 4002, Level = LogLevel.Error, Message = "Invalid configuration at '{position}'")]
    public static partial void InvalidConfiguration(this ILogger logger, ValidationException exception, string position);

    [LoggerMessage(EventId = 4003, Level = LogLevel.Trace, Message = "{count} configurations found")]
    public static partial void ConfigurationFound(this ILogger logger, int count);

    [LoggerMessage(EventId = 4004, Level = LogLevel.Trace, Message = "Consumer group not specified")]
    public static partial void ConsumerGroupNotPresent(this ILogger logger);

    [LoggerMessage(EventId = 4006, Level = LogLevel.Trace, Message = "Error on start")]
    public static partial void ErrorOnStart(this ILogger logger, Exception exception);

    [LoggerMessage(EventId = 4007, Level = LogLevel.Trace, Message = "Restarting '{key}' ...")]
    public static partial void RestartingByKey(this ILogger logger, string key);

    [LoggerMessage(EventId = 4007, Level = LogLevel.Trace, Message = "Restarted '{key}'")]
    public static partial void RestartedWithKey(this ILogger logger, string key);

    [LoggerMessage(EventId = 4009, Level = LogLevel.Trace, Message = "Starting period produce job on '{infobaseAddress}' ({start} - {end})")]
    public static partial void StartingPeriodProduceJob(this ILogger logger, string infobaseAddress, DateTimeOffset start, DateTimeOffset end);

    [LoggerMessage(EventId = 4010, Level = LogLevel.Trace, Message = "Starting watching infobase '{infobaseAddress}' changes")]
    public static partial void StartingWatchingChanges(this ILogger logger, string infobaseAddress);

    [LoggerMessage(EventId = 4011, Level = LogLevel.Trace, Message = "Processing infobase '{infobaseAddress}' change")]
    public static partial void ProcessingInfobaseChange(this ILogger logger, string infobaseAddress);

    [LoggerMessage(EventId = 4012, Level = LogLevel.Trace, Message = "Request infobase '{infobaseAddress}' changes")]
    public static partial void RequestInfobaseChanges(this ILogger logger, string infobaseAddress);

    [LoggerMessage(EventId = 4015, Level = LogLevel.Trace, Message = "'{consumerGroup}' consuming message from '{topicName}' (key: '{key}', length: {length}): {shortenedMessage}")]
    public static partial void ConsumingMessage(this ILogger logger, string consumerGroup, string topicName, string key, int length, string shortenedMessage);

    [LoggerMessage(EventId = 4012, Level = LogLevel.Warning, Message = "'{infobaseAddress}' producer errors exceeded")]
    public static partial void ErrorsExceeded(this ILogger logger, string infobaseAddress);

    [LoggerMessage(EventId = 4019, Level = LogLevel.Trace, Message = "Getting transactions from infobase '{infobaseAddress}' for {start} - {end}")]
    public static partial void GettingTransactionsForPeriod(this ILogger logger, string infobaseAddress, DateTimeOffset start, DateTimeOffset end);

    [LoggerMessage(EventId = 4020, Level = LogLevel.Trace, Message = "Getting new transactions from infobase '{infobaseAddress}'")]
    public static partial void GettingNewTransactions(this ILogger logger, string infobaseAddress);

    [LoggerMessage(EventId = 4021, Level = LogLevel.Trace, Message = "{transactionsCount} transactions found")]
    public static partial void TransactionsFound(this ILogger logger, int transactionsCount);

    [LoggerMessage(EventId = 4030, Level = LogLevel.Trace, Message = "Getting objects from infobase '{infobaseAddress}'")]
    public static partial void GettingObjects(this ILogger logger, string infobaseAddress);

    [LoggerMessage(EventId = 4031, Level = LogLevel.Trace, Message = "{objectsCount} objects retrieved")]
    public static partial void GotObjects(this ILogger logger, int objectsCount);

    [LoggerMessage(EventId = 4036, Level = LogLevel.Trace, Message = "Interpreter result is {objectsCount} object")]
    public static partial void JsonTransformResult(this ILogger logger, int objectsCount);

    [LoggerMessage(EventId = 4037, Level = LogLevel.Error, Message = "Interpreter error")]
    public static partial void JsonTransformError(this ILogger logger, Exception exception);

    [LoggerMessage(EventId = 4040, Level = LogLevel.Trace, Message = "Saving {objectsCount} objects")]
    public static partial void SavingObjects(this ILogger logger, int objectsCount);

    [LoggerMessage(EventId = 4041, Level = LogLevel.Trace, Message = "Sending {objectsCount} objects")]
    public static partial void SendingObjectJsons(this ILogger logger, int objectsCount);

    [LoggerMessage(EventId = 4042, Level = LogLevel.Trace, Message = "Sending object to topic '{topicName}': '{shortenedMessage}'")]
    public static partial void SendingObjectJson(this ILogger logger, string topicName, string shortenedMessage);

    [LoggerMessage(EventId = 4079, Level = LogLevel.Trace, Message = "Cancelling job on '{infobaseAddress}' ({start} - {end})")]
    public static partial void CancellingPeriodProduceJob(this ILogger logger, string infobaseAddress, DateTimeOffset start, DateTimeOffset end);

    [LoggerMessage(EventId = 4080, Level = LogLevel.Trace, Message = "Stopping {producersCount} producers")]
    public static partial void StoppingProducers(this ILogger logger, int producersCount);

    [LoggerMessage(EventId = 4081, Level = LogLevel.Trace, Message = "Stopping {consumersCount} consumers")]
    public static partial void StoppingConsumers(this ILogger logger, int consumersCount);

    [LoggerMessage(EventId = 5000, Level = LogLevel.Error, Message = "Error on produce job on '{infobaseAddress}' ({start} - {end})")]
    public static partial void ErrorOnPeriodProduceJob(this ILogger logger, Exception exception, string infobaseAddress, DateTimeOffset start, DateTimeOffset end);

    [LoggerMessage(EventId = 5001, Level = LogLevel.Error, Message = "Error on infobase changes")]
    public static partial void ErrorOnInfobaseChange(this ILogger logger, Exception exception);

    [LoggerMessage(EventId = 5002, Level = LogLevel.Warning, Message = "Operation cancelled")]
    public static partial void OperationCancelled(this ILogger logger);

    [LoggerMessage(EventId = 5011, Level = LogLevel.Trace, Message = "Period produce job on '{infobaseAddress}' ({start} - {end}) finished")]
    public static partial void PeriodProduceJobFinished(this ILogger logger, string infobaseAddress, DateTimeOffset start, DateTimeOffset end);

    [LoggerMessage(EventId = 5012, Level = LogLevel.Warning, Message = "Period produce job on '{infobaseAddress}' ({start} - {end}) operation cancelled")]
    public static partial void PeriodProduceJobCancelled(this ILogger logger, string infobaseAddress, DateTimeOffset start, DateTimeOffset end);

    [LoggerMessage(EventId = 5023, Level = LogLevel.Trace, Message = "Disposing '{infobaseAddress}'")]
    public static partial void Disposing(this ILogger logger, string infobaseAddress);

    [LoggerMessage(EventId = 5034, Level = LogLevel.Warning, Message = "Already disposed '{infobaseAddress}'")]
    public static partial void AlreadyDisposed(this ILogger logger, string infobaseAddress);

    [LoggerMessage(EventId = 5045, Level = LogLevel.Trace, Message = "Disposing job '{infobaseAddress}' ({start} - {end})")]
    public static partial void DisposingProduceJob(this ILogger logger, string infobaseAddress, DateTimeOffset start, DateTimeOffset end);

    [LoggerMessage(EventId = 5057, Level = LogLevel.Trace, Message = "Disposed {disposedCount} jobs")]
    public static partial void DisposedProduceJobs(this ILogger logger, int disposedCount);

    [LoggerMessage(EventId = 7001, Level = LogLevel.Warning, Message = "Invalid offset format: '{offset}'")]
    public static partial void InvalidOffsetFormat(this ILogger logger, string offset);

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

        ReadOnlySpan<char> messageSpan = message.AsSpan();

        StringBuilder stringBuilder = new();

        int leftOffset = (messageSpan.Length - startEndSeparator.Length) / 2;
        stringBuilder.Append(messageSpan.Slice(0, leftOffset));

        stringBuilder.Append(startEndSeparator);

        int rightOffset = (messageSpan.Length + startEndSeparator.Length) / 2;

        if (stringBuilder.Length + (messageSpan.Length - rightOffset) > lengthLimit)
        {
            rightOffset++;
        }

        stringBuilder.Append(messageSpan.Slice(rightOffset));

        return stringBuilder.ToString();
    }
}
