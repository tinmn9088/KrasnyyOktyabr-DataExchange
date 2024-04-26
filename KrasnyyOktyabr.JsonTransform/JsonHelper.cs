using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform;

public static class JsonHelper
{
    private static readonly Regex s_arrayIndexRegex = new(@"^\[(?<index>\d+)\]$");

    /// <summary>
    /// Deletes all properties with values that match (<see cref="string.IsNullOrWhiteSpace(string)"/>).
    /// </summary>
    public static void RemoveEmptyProperties(JToken json)
    {
        if (json is JObject jsonObject)
        {
            string[] keys = jsonObject.Properties().Select(p => p.Name).ToArray();
            foreach (string key in keys)
            {
                JToken propertyValue = jsonObject[key]!;

                if (propertyValue is JValue primitiveValue)
                {
                    if (primitiveValue.Value == null)
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

    /// <exception cref="ArgumentNullException"></exception>
    public static JObject Unflatten(JObject flatJson)
    {
        if (flatJson == null)
        {
            throw new ArgumentNullException(nameof(flatJson));
        }

        static JToken wrap(JToken? flatJsonPropertyValue, string[] keyParts, int keyPartIndex = 0)
        {
            JToken? value = keyPartIndex + 1 >= keyParts.Length
                ? flatJsonPropertyValue
                : wrap(flatJsonPropertyValue, keyParts, keyPartIndex + 1);

            string keyPart = keyParts[keyPartIndex];

            Match arrayIndexMatch = s_arrayIndexRegex.Match(keyPart);

            if (arrayIndexMatch.Success)
            {
                int arrayIndex = int.Parse(arrayIndexMatch.Groups["index"].Value);

                JArray array = [];

                for (int i = 0; i < arrayIndex; i++)
                {
                    array.Add(JValue.CreateNull());
                }

                array.Add(value ?? JValue.CreateNull());

                return array;
            }
            else
            {
                return new JObject()
                {
                    { keyPart, value }
                };
            }
        }

        JObject structuredJson = new();

        foreach (KeyValuePair<string, JToken?> flatJsonProperty in flatJson)
        {
            string flatJsonPropertyKey = flatJsonProperty.Key;
            JToken? flatJsonPropertyValue = flatJsonProperty.Value;

            flatJsonPropertyKey = flatJsonPropertyKey.Replace("[", ".[");

            string[] keyParts = flatJsonPropertyKey.Split('.');

            JToken structuredJsonPart = wrap(flatJsonPropertyValue, keyParts);

            structuredJson.Merge(structuredJsonPart, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Merge
            });
        }

        return structuredJson;
    }

    /// <exception cref="ArgumentNullException"></exception>
    public static JObject Flatten(JObject structuredJson, string keyPrefix = "")
    {
        if (structuredJson == null)
        {
            throw new ArgumentNullException(nameof(structuredJson));
        }

        JObject flatJson = new();

        foreach (KeyValuePair<string, JToken?> structuredJsonProperty in structuredJson)
        {
            string prefixedKey = keyPrefix.Length == 0
                    ? structuredJsonProperty.Key
                    : $"{keyPrefix}.{structuredJsonProperty.Key}";

            if (structuredJsonProperty.Value is JObject nestedObject)
            {
                JObject nestedFlatJson = Flatten(nestedObject, prefixedKey);
                flatJson.Merge(nestedFlatJson);
            }
            else if (structuredJsonProperty.Value is JArray nestedArray)
            {
                JObject arrayFlatJson = FlattenArray(nestedArray, prefixedKey);
                flatJson.Merge(arrayFlatJson);
            }
            else
            {
                flatJson.Add(prefixedKey, structuredJsonProperty.Value);
            }
        }

        return flatJson;
    }

    public static JObject FlattenArray(JArray array, string keyPrefix = "")
    {
        JObject flatJson = new();

        int nestedArrayCount = array.Count;
        for (int i = 0; i < nestedArrayCount; i++)
        {
            string arrayItemPrefixedKey = keyPrefix.Length == 0
                ? $"[{i}]"
                : $"{keyPrefix}[{i}]";
            if (array[i] is JObject arrayItemObject)
            {
                JObject arrayItemFlatJson = Flatten(arrayItemObject, arrayItemPrefixedKey);
                flatJson.Merge(arrayItemFlatJson);
            }
            else
            {
                flatJson.Add(arrayItemPrefixedKey, array[i]);
            }
        }

        return flatJson;
    }
}
