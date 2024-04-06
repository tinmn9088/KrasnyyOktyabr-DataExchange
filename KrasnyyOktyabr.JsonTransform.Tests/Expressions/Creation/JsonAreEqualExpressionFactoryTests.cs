using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonAreEqualExpressionFactory;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonAreEqualExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonAreEqualExpressionFactory? _areEqualExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _areEqualExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyEq,
                new JObject()
                {
                    { JsonSchemaPropertyLeft, null },
                    { JsonSchemaPropertyRight, null },
                }
            },
        };

        bool isMatch = _areEqualExpressionFactory!.Match(input);

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
                JsonSchemaPropertyEq,
                new JObject()
                {
                    { JsonSchemaPropertyLeft, null },
                    { JsonSchemaPropertyRight, null },
                }
            },
        };

        bool isMatch = _areEqualExpressionFactory!.Match(input);

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
                JsonSchemaPropertyEq,
                new JObject()
                {
                    { JsonSchemaPropertyLeft, null },
                    { JsonSchemaPropertyRight, null },
                }
            },
        };

        bool isMatch = _areEqualExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _areEqualExpressionFactory!.Create(null!);
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
                JsonSchemaPropertyEq,
                new JObject()
                {
                    { JsonSchemaPropertyLeft, fakeLeftInstruction },
                    { JsonSchemaPropertyRight, fakeRightInstruction },
                }
            },
        };

        AreEqualExpression expression = _areEqualExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task<object?>>>(It.Is<JToken>(i => i == fakeLeftInstruction)), Times.Once);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task<object?>>>(It.Is<JToken>(i => i == fakeRightInstruction)), Times.Once);
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
