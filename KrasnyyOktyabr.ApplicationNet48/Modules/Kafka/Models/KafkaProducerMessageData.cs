using System;
using System.Collections.Generic;
using KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.HelperServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.Models;

public struct KafkaProducerMessageData
{
    public KafkaProducerMessageData(string objectJson, string? dataType)
    {
        ObjectJson = objectJson;
        DataType = dataType;
    }

    public KafkaProducerMessageData(
        string objectJson,
        Dictionary<string, object?> propertiesToAdd,
        string? dataTypePropertyName = null)
    {
        var jsonUtil = new JsonService();

        JObject jObject = jsonUtil.ParseObjectJson(objectJson);

        jsonUtil.RemoveEmptyProperties(jObject);

        jsonUtil.AddProperties(jObject, propertiesToAdd);

        if (dataTypePropertyName is not null)
        {
            var dataTypeJToken =
                jObject[dataTypePropertyName] ??
                throw new FailedToGetDataTypeException(dataTypePropertyName);

            var dataTypeString =
                dataTypeJToken.Value<string>() ??
                throw new FailedToGetDataTypeException(dataTypePropertyName);

            DataType = dataTypeString;
        }

        ObjectJson = jObject.ToString(Formatting.None);
    }

    public readonly string ObjectJson { get; }

    public readonly string? DataType { get; } = null;

    public string? TopicName { get; set; } = null;
}

public class FailedToGetDataTypeException : Exception
{
    internal FailedToGetDataTypeException(string dataTypePropertyName)
        : base($"Failed to get data type with property name '{dataTypePropertyName}'")
    {
    }
}