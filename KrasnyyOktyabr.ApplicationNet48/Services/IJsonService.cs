using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KrasnyyOktyabr.ApplicationNet48.Services;

public interface IJsonService
{
    public readonly struct JsonTransformMsSqlResult(string table, Dictionary<string, dynamic> columnValues)
    {
        public string Table { get; } = table;

        public Dictionary<string, dynamic> ColumnValues { get; } = columnValues;
    }

    public readonly struct KafkaProducerMessageData(string objectJson, string dataType)
    {
        public string ObjectJson { get; } = objectJson;

        public string DataType { get; } = dataType;
    }

    int ClearCachedExpressions();

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
    /// <remarks>
    /// <paramref name="outputStream"/> may be written synchronously.
    /// </remarks>
    /// <exception cref="Exception"></exception>
    ValueTask RunJsonTransformAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken);

    /// <exception cref="Exception"></exception>
    ValueTask<List<JsonTransformMsSqlResult>> RunJsonTransformOnConsumedMessageMsSqlAsync(
        string instructionName,
        string message,
        string tablePropertyName,
        CancellationToken cancellationToken);

    /// <exception cref="Exception"></exception>
    ValueTask<List<string>> RunJsonTransformOnConsumedMessageVApplicationAsync(
        string instructionName,
        string message,
        CancellationToken cancellationToken);

    public class FailedToGetDataTypeException : Exception
    {
        internal FailedToGetDataTypeException(string dataTypePropertyName)
            : base($"Failed to get data type with property name '{dataTypePropertyName}'")
        {
        }
    }
}
