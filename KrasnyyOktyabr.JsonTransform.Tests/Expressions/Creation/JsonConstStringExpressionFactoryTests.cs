using KrasnyyOktyabr.JsonTransform.Expressions;
using KrasnyyOktyabr.JsonTransform.Expressions.Creation;
using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Tests.Expressions.Creation;

[TestClass]
public class JsonConstStringExpressionFactoryTests
{
    private static readonly JsonConstStringExpressionFactory s_constStringExpressionFactory = new();

    [TestMethod]
    public void Create_ShouldCreateConstStringExpression()
    {
        JToken input = "TestString";

        ConstStringExpression expression = s_constStringExpressionFactory.Create(input);

        Assert.IsNotNull(expression);
    }

    [TestMethod]
    public void Match_WhenInputInvalid_ShouldReturnFalse()
    {
        JObject input = new()
        {
            { "IllegalProperty", null },
        };

        bool isMatch = s_constStringExpressionFactory.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JToken input = "TestString";

        bool isMatch = s_constStringExpressionFactory.Match(input);

        Assert.IsTrue(isMatch);
    }
}
