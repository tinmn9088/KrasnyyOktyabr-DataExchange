using KrasnyyOktyabr.JsonTransform.Expressions;
using KrasnyyOktyabr.JsonTransform.Expressions.Creation;
using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Tests.Expressions.Creation;

[TestClass]
public class JsonConstBoolExpressionFactoryTests
{
    private static readonly JsonConstBoolExpressionFactory s_constBoolExpressionFactory = new();

    [TestMethod]
    public void Create_ShouldCreateConstBoolExpression()
    {
        JToken input = true;

        ConstBoolExpression expression = s_constBoolExpressionFactory.Create(input);

        Assert.IsNotNull(expression);
    }

    [TestMethod]
    public void Match_WhenInputInvalid_ShouldReturnFalse()
    {
        JObject input = new()
        {
            { "IllegalProperty", null },
        };

        bool isMatch = s_constBoolExpressionFactory.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JToken input = false;

        bool isMatch = s_constBoolExpressionFactory.Match(input);

        Assert.IsTrue(isMatch);
    }
}
