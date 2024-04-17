namespace KrasnyyOktyabr.Application.Services;

public interface IJsonService
{
    public readonly struct RunJsonTransformMsSqlResult
    {
        public required string Table { get; init; }

        public required Dictionary<string, dynamic> ColumnValues { get; init; }
    }

    public readonly struct KafkaProducerMessageData
    {
        public required string ObjectJson { get; init; }

        public required string DataType { get; init; }
    }

    public static string InstructionsPropertyName => "instructions";

    public static string InputPropertyName => "input";

    /// <summary>
    /// Remove empty properties using <see cref="JsonTransform.JsonHelper"/>,
    /// add properties from <paramref name="propertiesToAdd"/>
    /// and extract property with name <paramref name="dataTypePropertyName"/>.
    /// </summary>
    /// <param name="dataType"></param>
    /// <exception cref="FailedToGetDataTypeException"></exception>
    KafkaProducerMessageData BuildKafkaProducerMessageData(string objectJson, Dictionary<string, object?> propertiesToAdd, string dataTypePropertyName);

    /// <param name="inputStream">
    /// Must contain JSON: <c>"{'instructions': ... ,'input': { ... } }"</c>
    /// </param>
    /// <exception cref="Exception"></exception>
    Task RunJsonTransformAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken);

    /// <exception cref="Exception"></exception>
    Task<RunJsonTransformMsSqlResult> RunJsonTransformOnConsumedMessageMsSqlAsync(string producerName, string consumerName, string message, CancellationToken cancellationToken);

    public class FailedToGetDataTypeException : Exception
    {
        internal FailedToGetDataTypeException(string dataTypePropertyName)
            : base($"Failed to get data type with property name '{dataTypePropertyName}'")
        {
        }
    }
}
