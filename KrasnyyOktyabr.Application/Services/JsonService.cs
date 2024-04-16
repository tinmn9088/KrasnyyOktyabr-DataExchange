using KrasnyyOktyabr.JsonTransform;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.Application.Services.IJsonService;

namespace KrasnyyOktyabr.Application.Services;

public sealed class JsonService : IJsonService
{
    public V77ApplicationProducerMessageData BuildV77ApplicationProducerMessageData(
        string objectJson,
        Dictionary<string, object?> propertiesToAdd,
        string dataTypePropertyName)
    {
        ArgumentNullException.ThrowIfNull(objectJson);
        ArgumentNullException.ThrowIfNull(propertiesToAdd);
        ArgumentNullException.ThrowIfNull(dataTypePropertyName);

        JObject jObject = ParseJsonObject(objectJson);

        JsonHelper.RemoveEmptyProperties(jObject);

        AddProperties(jObject, propertiesToAdd);

        string dataType = (jObject[dataTypePropertyName] ?? throw new FailedToGetDataTypeException(dataTypePropertyName))
            .Value<string>() ?? throw new FailedToGetDataTypeException(dataTypePropertyName);

        return new()
        {
            ObjectJson = jObject.ToString(Formatting.None),
            DataType = dataType,
        };
    }

    /// <exception cref="ArgumentException"></exception>
    private static JObject ParseJsonObject(string jsonObject)
    {
        ArgumentNullException.ThrowIfNull(jsonObject);

        try
        {
            return JObject.Parse(jsonObject);
        }
        catch (JsonReaderException ex)
        {
            throw new ArgumentException("Failed to parse JSON", ex);
        }
    }

    private static void AddProperties(JObject jObject, Dictionary<string, object?> propertiesToAdd)
    {
        foreach (KeyValuePair<string, object?> property in propertiesToAdd)
        {
            jObject[property.Key] = JToken.FromObject(property.Value ?? JValue.CreateNull());
        }
    }
}
