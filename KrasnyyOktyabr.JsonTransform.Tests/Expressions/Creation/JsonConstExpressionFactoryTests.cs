using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonConstExpressionFactory;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonConstExpressionFactoryTests
{
    private static readonly JsonConstExpressionFactory s_constExpressionFactory = new();

    [TestMethod]
    public void Create_ShouldCreateConstExpression()
    {
        JObject input = new()
        {
            { JsonSchemaPropertyConst, "TestValue" },
        };

        ConstExpression expression = s_constExpressionFactory.Create(input);

        Assert.IsNotNull(expression);
    }

    [TestMethod]
    public void Match_WhenInputInvalid_ShouldReturnFalse()
    {
        JObject input = new()
        {
            { "IllegalProperty", null },
        };

        bool isMatch = s_constExpressionFactory.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            { JsonSchemaPropertyConst, "TestValue" },
        };

        bool isMatch = s_constExpressionFactory.Match(input);

        Assert.IsTrue(isMatch);
    }

    [TestMethod]
    public void Match_WhenInputWithComment_ShouldReturnTrue()
    {
        JObject input = new()
        {
            { JsonSchemaPropertyConst, "TestValue" },
            { JsonSchemaPropertyComment, "TestComment" },
        };

        bool isMatch = s_constExpressionFactory.Match(input);

        Assert.IsTrue(isMatch);
    }
}
