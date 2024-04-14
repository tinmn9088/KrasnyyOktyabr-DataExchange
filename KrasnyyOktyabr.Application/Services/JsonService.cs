using KrasnyyOktyabr.JsonTransform;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.Application.Services;

public class JsonService : IJsonService
{
    public string RemoveEmptyPropertiesAndAdd(string jsonObject, Dictionary<string, object?> propertiesToAdd)
    {
        ArgumentNullException.ThrowIfNull(jsonObject);
        ArgumentNullException.ThrowIfNull(propertiesToAdd);

        JObject jObject = ParseJsonObject(jsonObject);

        JsonHelper.RemoveEmptyProperties(jObject);

        return AddPropertiesAndSerialize(jObject, propertiesToAdd);
    }

    /// <exception cref="ArgumentException"></exception>
    private static JObject ParseJsonObject(string jsonObject)
    {
        try
        {
            return JObject.Parse(jsonObject);
        }
        catch (JsonReaderException ex)
        {
            throw new ArgumentException("Failed to parse JSON", ex);
        }
    }

    private static string AddPropertiesAndSerialize(JObject jObject, Dictionary<string, object?> propertiesToAdd)
    {
        foreach (KeyValuePair<string, object?> property in propertiesToAdd)
        {
            jObject[property.Key] = JToken.FromObject(property.Value ?? JValue.CreateNull());
        }

        return jObject.ToString(Formatting.None);
    }
}
