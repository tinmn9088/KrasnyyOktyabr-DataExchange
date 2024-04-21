using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.Application.Services;

public static class JsonHelper
{
    public static bool TryConvertThroughJToken<T>(object value, out T? converted)
    {
        converted = default;

        T? convertedValue = JToken.FromObject(value).ToObject<T>();

        if (convertedValue != null)
        {
            converted = convertedValue;

            return true;
        }

        return false;
    }
}
