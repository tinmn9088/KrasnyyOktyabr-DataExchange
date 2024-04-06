using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonOrExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonOrExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonOrExpressionFactory? _orExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _orExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyOr,
                new JObject()
                {
                    { JsonSchemaPropertyLeft, null },
                    { JsonSchemaPropertyRight, null },
                }
            },
        };

        bool isMatch = _orExpressionFactory!.Match(input);

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
                JsonSchemaPropertyOr,
                new JObject()
                {
                    { JsonSchemaPropertyLeft, null },
                    { JsonSchemaPropertyRight, null },
                }
            },
        };

        bool isMatch = _orExpressionFactory!.Match(input);

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
                JsonSchemaPropertyOr,
                new JObject()
                {
                    { JsonSchemaPropertyLeft, null },
                    { JsonSchemaPropertyRight, null },
                }
            },
        };

        bool isMatch = _orExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _orExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateOrExpression()
    {
        // Setting up left instruction mock
        JObject fakeLeftInstruction = new();
        Mock<IExpression<Task<bool>>> leftExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<bool>>>(fakeLeftInstruction))
            .Returns(leftExpressionMock.Object);

        // Setting up right instruction mock
        JObject fakeRightInstruction = new();
        Mock<IExpression<Task<bool>>> rightExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<bool>>>(fakeRightInstruction))
            .Returns(rightExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertyOr,
                new JObject()
                {
                    { JsonSchemaPropertyLeft, fakeLeftInstruction },
                    { JsonSchemaPropertyRight, fakeRightInstruction },
                }
            },
        };

        OrExpression expression = _orExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task<bool>>>(It.Is<JToken>(i => i == fakeLeftInstruction)), Times.Once);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task<bool>>>(It.Is<JToken>(i => i == fakeRightInstruction)), Times.Once);
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
