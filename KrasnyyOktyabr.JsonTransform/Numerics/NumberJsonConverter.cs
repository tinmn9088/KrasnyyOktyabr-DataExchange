using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Numerics;

public sealed class NumberJsonConverter : JsonConverter<Number>
{
    public override bool CanRead => false;

    public override Number ReadJson(JsonReader reader, Type objectType, Number existingValue, bool hasExistingValue, JsonSerializer serializer) => throw new NotImplementedException();

    public override void WriteJson(JsonWriter writer, Number value, JsonSerializer serializer)
    {
        if (value.Long != null)
        {
            JToken.FromObject(value.Long).WriteTo(writer);
        }

        if (value.Decimal != null)
        {
            JToken.FromObject(value.Decimal).WriteTo(writer);
        }
    }
}
