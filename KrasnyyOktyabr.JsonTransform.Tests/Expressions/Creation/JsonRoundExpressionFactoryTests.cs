using KrasnyyOktyabr.JsonTransform.Numerics;
using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonRoundExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonRoundExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonRoundExpressionFactory? _roundExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _roundExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyRound,
                new JObject()
                {
                    { JsonSchemaPropertyValue, null },
                    { JsonSchemaPropertyDigits, 0 },
                }
            },
        };

        bool isMatch = _roundExpressionFactory!.Match(input);

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
                JsonSchemaPropertyRound,
                new JObject()
                {
                    { JsonSchemaPropertyValue, null },
                }
            },
        };

        bool isMatch = _roundExpressionFactory!.Match(input);

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
                JsonSchemaPropertyRound,
                new JObject()
                {
                    { JsonSchemaPropertyValue, null },
                }
            },
        };

        bool isMatch = _roundExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _roundExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateRoundExpression()
    {
        // Setting up value instruction mock
        JObject fakeValueInstruction = new();
        JToken fakeDigitsInstruction = 2;
        Mock<IExpression<Task<Number>>> valueExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<Number>>>(fakeValueInstruction))
            .Returns(valueExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertyRound,
                new JObject()
                {
                    { JsonSchemaPropertyValue, fakeValueInstruction },
                    { JsonSchemaPropertyDigits, fakeDigitsInstruction },
                }
            },
        };

        RoundExpression expression = _roundExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task<Number>>>(It.Is<JToken>(i => i == fakeValueInstruction)), Times.Once);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task<int>>>(It.Is<JToken>(i => i == fakeDigitsInstruction)), Times.Once);
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
