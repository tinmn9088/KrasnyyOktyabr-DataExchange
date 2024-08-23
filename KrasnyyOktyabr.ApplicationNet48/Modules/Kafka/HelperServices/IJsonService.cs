using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.HelperServices;

public interface IJsonService
{
    /// <summary>
    /// Works with <see cref="Newtonsoft.Json.JsonPropertyAttribute"/>.
    /// </summary>
    string Serialize(object item);

    /// <remarks>
    /// Uses <c>null</c> when property is not present.
    /// </remarks>
    Dictionary<string, string?> ExtractProperties(string json, IEnumerable<string> propertyNames);

    public JObject ParseObjectJson(string objectJson);

    public void AddProperties(JObject jObject, Dictionary<string, object?> propertiesToAdd);

    public void RemoveEmptyProperties(JToken json);

}