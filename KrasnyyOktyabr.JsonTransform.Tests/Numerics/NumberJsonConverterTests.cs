using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Numerics.Tests;

[TestClass]
public class NumberJsonConverterTests
{
    private static readonly JsonSerializer s_jsonSerializer = JsonSerializer.Create(new() { Converters = [new NumberJsonConverter()] });

    [TestMethod]
    public void WriteJson_ShouldWriteInt()
    {
        int expected = 4;
        Number number = new(expected);

        JToken actual = JToken.FromObject(number, s_jsonSerializer);

        Assert.AreEqual(JToken.FromObject(expected), actual);
    }

    [TestMethod]
    public void WriteJson_ShouldWriteDouble()
    {
        double expected = 5.5;
        Number number = new(expected);

        JToken actual = JToken.FromObject(number, s_jsonSerializer);

        Assert.AreEqual(JToken.FromObject(expected), actual);
    }
}
