namespace KrasnyyOktyabr.Application.Services;

public interface IJsonService
{
    public readonly struct V77ApplicationProducerMessageData
    {
        public required string ObjectJson { get; init; }

        public required string DataType { get; init; }
    }

    /// <summary>
    /// Remove empty properties using <see cref="JsonTransform.JsonHelper"/>,
    /// add properties from <paramref name="propertiesToAdd"/>
    /// and extract property with name <paramref name="dataTypePropertyName"/>.
    /// </summary>
    /// <param name="dataType"></param>
    /// <exception cref="FailedToGetDataTypeException"></exception>
    V77ApplicationProducerMessageData BuildV77ApplicationProducerMessageData(string objectJson, Dictionary<string, object?> propertiesToAdd, string dataTypePropertyName);

    public class FailedToGetDataTypeException : Exception
    {
        internal FailedToGetDataTypeException(string dataTypePropertyName)
            : base($"Failed to get data type with property name '{dataTypePropertyName}'")
        {
        }
    }
}
