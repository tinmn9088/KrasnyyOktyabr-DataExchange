
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.HelperServices;

public class JsonService : IJsonService
{
    public string Serialize(object item)
    {
        JsonSerializer serializer = JsonSerializer.CreateDefault();

        using TextWriter writer = new StringWriter();

        serializer.Serialize(writer, item);
        return writer.ToString();
    }
    
    public Dictionary<string, string?> ExtractProperties(string json, IEnumerable<string> propertyNames)
    {
        JToken jToken = JToken.Parse(json);

        Dictionary<string, string?> extractedValues = [];

        foreach (string propertyName in propertyNames)
        {
            string? extractedValue = jToken[propertyName]?.Type == JTokenType.Null
                ? null
                : jToken[propertyName]?.ToString();

            extractedValues[propertyName] = extractedValue;
        }

        return extractedValues;
    }
    
    /// <exception cref="ArgumentException"></exception>
    public JObject ParseObjectJson(string objectJson)
    {
        try
        {
            return JObject.Parse(objectJson);
        }
        catch (JsonReaderException ex)
        {
            throw new ArgumentException("Failed to parse JSON", ex);
        }
    }
    
    public void AddProperties(JObject jObject, Dictionary<string, object?> propertiesToAdd)
    {
        foreach (KeyValuePair<string, object?> property in propertiesToAdd)
        {
            jObject[property.Key] = JToken.FromObject(property.Value ?? JValue.CreateNull());
        }
    }
    
    /// <summary>
    /// Deletes all properties with values that match (<see cref="string.IsNullOrWhiteSpace(string)"/>).
    /// </summary>
    public void RemoveEmptyProperties(JToken json)
    {
        if (json is JObject jsonObject)
        {
            string[] keys = jsonObject.Properties().Select(p => p.Name).ToArray();
            foreach (string key in keys)
            {
                JToken propertyValue = jsonObject[key]!;

                if (propertyValue is JValue primitiveValue)
                {
                    if (primitiveValue.Value is null)
                    {
                        jsonObject.Remove(key);
                    }

                    if (primitiveValue.Value is string stringValue)
                    {
                        if (string.IsNullOrWhiteSpace(stringValue))
                        {
                            jsonObject.Remove(key);
                        }
                    }
                }

                if (propertyValue is JObject || propertyValue is JArray)
                {
                    RemoveEmptyProperties(propertyValue);
                }
            }
        }

        if (json is JArray array)
        {
            foreach (JToken arrayItem in array)
            {
                RemoveEmptyProperties(arrayItem);
            }
        }
    }
}