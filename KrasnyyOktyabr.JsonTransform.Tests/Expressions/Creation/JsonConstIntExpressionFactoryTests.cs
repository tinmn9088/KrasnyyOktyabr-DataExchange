using KrasnyyOktyabr.JsonTransform.Expressions;
using KrasnyyOktyabr.JsonTransform.Expressions.Creation;
using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Tests.Expressions.Creation;

[TestClass]
public class JsonConstIntExpressionFactoryTests
{
    private static readonly JsonConstIntExpressionFactory s_constIntExpressionFactory = new();

    [TestMethod]
    public void Create_ShouldCreateConstIntExpression()
    {
        JToken input = 66;

        ConstIntExpression expression = s_constIntExpressionFactory.Create(input);

        Assert.IsNotNull(expression);
    }

    [TestMethod]
    public void Match_WhenInputInvalid_ShouldReturnFalse()
    {
        JObject input = new()
        {
            { "IllegalProperty", null },
        };

        bool isMatch = s_constIntExpressionFactory.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JToken input = 99;

        bool isMatch = s_constIntExpressionFactory.Match(input);

        Assert.IsTrue(isMatch);
    }
}
