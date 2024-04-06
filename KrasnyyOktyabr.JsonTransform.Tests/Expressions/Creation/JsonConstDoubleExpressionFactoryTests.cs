using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonConstDoubleExpressionFactoryTests
{
    private static readonly JsonConstDoubleExpressionFactory s_constDoubleExpressionFactory = new();

    [TestMethod]
    public void Create_ShouldCreateConstDoubleExpression()
    {
        JToken input = 66.6;

        ConstDoubleExpression expression = s_constDoubleExpressionFactory.Create(input);

        Assert.IsNotNull(expression);
    }

    [TestMethod]
    public void Match_WhenInputInvalid_ShouldReturnFalse()
    {
        JObject input = new()
        {
            { "IllegalProperty", null },
        };

        bool isMatch = s_constDoubleExpressionFactory.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JToken input = 99.9;

        bool isMatch = s_constDoubleExpressionFactory.Match(input);

        Assert.IsTrue(isMatch);
    }
}
