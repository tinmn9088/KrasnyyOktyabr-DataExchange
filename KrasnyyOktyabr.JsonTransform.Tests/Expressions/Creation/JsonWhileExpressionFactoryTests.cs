using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonWhileExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonWhileExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonWhileExpressionFactory? _whileExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _whileExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyWhile,
                new JObject()
                {
                    { JsonSchemaPropertyCondition, null },
                    { JsonSchemaPropertyInstructions, null },
                }
            },
        };

        bool isMatch = _whileExpressionFactory!.Match(input);

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
                JsonSchemaPropertyWhile,
                new JObject()
                {
                    { JsonSchemaPropertyCondition, null },
                    { JsonSchemaPropertyInstructions, null },
                }
            },
        };

        bool isMatch = _whileExpressionFactory!.Match(input);

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
                JsonSchemaPropertyWhile,
                new JObject()
                {
                    { JsonSchemaPropertyCondition, null },
                    { JsonSchemaPropertyInstructions, null },
                }
            },
        };

        bool isMatch = _whileExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _whileExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateWhileExpression()
    {
        // Setting up name instruction mock
        JObject fakeConditionInstruction = new();
        Mock<IExpression<Task<bool>>> conditionExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<bool>>>(It.Is<JObject>(i => i == fakeConditionInstruction)))
            .Returns(conditionExpressionMock.Object);

        // Setting up instructions mock
        JObject fakeInstructions = new();
        Mock<IExpression<Task>> innerExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task>>(It.Is<JObject>(i => i == fakeInstructions)))
            .Returns(innerExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertyWhile,
                new JObject()
                {
                    { JsonSchemaPropertyCondition, fakeConditionInstruction },
                    { JsonSchemaPropertyInstructions, fakeInstructions },
                }
            },
        };

        WhileExpression expression = _whileExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(2));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
