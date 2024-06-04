#nullable enable

using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.ApplicationNet48.Services;

public static class JsonHelper
{
    public static bool TryConvertThroughJToken<T>(object value, out T? converted)
    {
        converted = default;

        T? convertedValue = JToken.FromObject(value).ToObject<T>();

        if (convertedValue is not null)
        {
            converted = convertedValue;

            return true;
        }

        return false;
    }
}
