using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Numerics;

public sealed class NumberJsonConverter : JsonConverter<Number>
{
    public override bool CanRead => false;

    public override Number ReadJson(JsonReader reader, Type objectType, Number existingValue, bool hasExistingValue, JsonSerializer serializer) => throw new NotImplementedException();

    public override void WriteJson(JsonWriter writer, Number value, JsonSerializer serializer)
    {
        if (value.Int != null)
        {
            JToken.FromObject(value.Int).WriteTo(writer);
        }

        if (value.Double != null)
        {
            JToken.FromObject(value.Double).WriteTo(writer);
        }
    }
}
