using KrasnyyOktyabr.JsonTransform.Numerics;
using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonMultiplyExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonMultiplyExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonMultiplyExpressionFactory? _multiplyExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _multiplyExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyMultiply,
                new JObject()
                {
                    { JsonSchemaPropertyLeft, null },
                    { JsonSchemaPropertyRight, null },
                }
            },
        };

        bool isMatch = _multiplyExpressionFactory!.Match(input);

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
                JsonSchemaPropertyMultiply,
                new JObject()
                {
                    { JsonSchemaPropertyLeft, null },
                    { JsonSchemaPropertyRight, null },
                }
            },
        };

        bool isMatch = _multiplyExpressionFactory!.Match(input);

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
                JsonSchemaPropertyMultiply,
                new JObject()
                {
                    { JsonSchemaPropertyLeft, null },
                    { JsonSchemaPropertyRight, null },
                }
            },
        };

        bool isMatch = _multiplyExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _multiplyExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateSumExpression()
    {
        // Setting up left instruction mock
        JObject fakeLeftInstruction = new();
        Mock<IExpression<Task<Number>>> leftExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<Number>>>(fakeLeftInstruction))
            .Returns(leftExpressionMock.Object);

        // Setting up right instruction mock
        JObject fakeRightInstruction = new();
        Mock<IExpression<Task<Number>>> rightExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<Number>>>(fakeRightInstruction))
            .Returns(rightExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertyMultiply,
                new JObject()
                {
                    { JsonSchemaPropertyLeft, fakeLeftInstruction },
                    { JsonSchemaPropertyRight, fakeRightInstruction },
                }
            },
        };

        MultiplyExpression expression = _multiplyExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task<Number>>>(It.Is<JToken>(i => i == fakeLeftInstruction)), Times.Once);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task<Number>>>(It.Is<JToken>(i => i == fakeRightInstruction)), Times.Once);
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
