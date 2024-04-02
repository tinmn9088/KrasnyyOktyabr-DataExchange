using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonStringFormatExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonStringFormatExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonStringFormatExpressionFactory? _stringFormatExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _stringFormatExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyStrformat,
                new JObject()
                {
                    { JsonSchemaPropertyValue, null },
                    { JsonSchemaPropertyArgs, new JArray() },
                }
            },
        };

        bool isMatch = _stringFormatExpressionFactory!.Match(input);

        Assert.IsTrue(isMatch);
    }

    [TestMethod]
    public void Match_WhenInputWithComment_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyComment, "TestComment"
            },
            {
                JsonSchemaPropertyStrformat,
                new JObject()
                {
                    { JsonSchemaPropertyValue, null },
                    { JsonSchemaPropertyArgs, new JArray() },
                }
            },
        };

        bool isMatch = _stringFormatExpressionFactory!.Match(input);

        Assert.IsTrue(isMatch);
    }

    [TestMethod]
    public void Match_WhenInputWithAdditionalProperties_ShouldReturnFalse()
    {
        JObject input = new()
        {
            {
                "AdditionalProperty", null
            },
            {
                JsonSchemaPropertyComment, "TestComment"
            },
            {
                JsonSchemaPropertyStrformat,
                new JObject()
                {
                    { JsonSchemaPropertyValue, null },
                    { JsonSchemaPropertyArgs, new JArray() },
                }
            },
        };

        bool isMatch = _stringFormatExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _stringFormatExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateStringFormatExpression()
    {
        // Setting up value instruction mock
        JObject fakeValueInstruction = new();
        Mock<IExpression<Task<string>>> valueExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<string>>>(fakeValueInstruction))
            .Returns(valueExpressionMock.Object);

        // Setting up arg0 instruction mock
        JObject fakeArg0Instruction = new();
        Mock<IExpression<Task<object?>>> arg0ExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<object?>>>(fakeArg0Instruction))
            .Returns(arg0ExpressionMock.Object);

        JObject input = new()
        {
            { JsonSchemaPropertyComment, "TestComment" },
            {
                JsonSchemaPropertyStrformat,
                new JObject()
                {
                    { JsonSchemaPropertyValue, fakeValueInstruction },
                    {
                        JsonSchemaPropertyArgs,
                        new JArray()
                        {
                            fakeArg0Instruction,
                        }
                    },
                }
            },
        };

        StringFormatExpression expression = _stringFormatExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(2));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
