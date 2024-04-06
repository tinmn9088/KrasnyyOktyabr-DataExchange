using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonAreNotEqualExpressionFactory;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonAreNotEqualExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonAreNotEqualExpressionFactory? _areNotEqualExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _areNotEqualExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyNeq,
                new JObject()
                {
                    { JsonSchemaPropertyLeft, null },
                    { JsonSchemaPropertyRight, null },
                }
            },
        };

        bool isMatch = _areNotEqualExpressionFactory!.Match(input);

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
                JsonSchemaPropertyNeq,
                new JObject()
                {
                    { JsonSchemaPropertyLeft, null },
                    { JsonSchemaPropertyRight, null },
                }
            },
        };

        bool isMatch = _areNotEqualExpressionFactory!.Match(input);

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
                JsonSchemaPropertyNeq,
                new JObject()
                {
                    { JsonSchemaPropertyLeft, null },
                    { JsonSchemaPropertyRight, null },
                }
            },
        };

        bool isMatch = _areNotEqualExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _areNotEqualExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateAreEqualExpression()
    {
        // Setting up left instruction mock
        JObject fakeLeftInstruction = new();
        Mock<IExpression<Task<object?>>> leftExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<object?>>>(fakeLeftInstruction))
            .Returns(leftExpressionMock.Object);

        // Setting up right instruction mock
        JObject fakeRightInstruction = new();
        Mock<IExpression<Task<object?>>> rightExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<object?>>>(fakeRightInstruction))
            .Returns(rightExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertyNeq,
                new JObject()
                {
                    { JsonSchemaPropertyLeft, fakeLeftInstruction },
                    { JsonSchemaPropertyRight, fakeRightInstruction },
                }
            },
        };

        AreNotEqualExpression expression = _areNotEqualExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task<object?>>>(It.Is<JToken>(i => i == fakeLeftInstruction)), Times.Once);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task<object?>>>(It.Is<JToken>(i => i == fakeRightInstruction)), Times.Once);
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
