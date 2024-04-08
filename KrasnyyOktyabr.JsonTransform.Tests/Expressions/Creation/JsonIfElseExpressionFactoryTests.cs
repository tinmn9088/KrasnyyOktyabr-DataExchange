using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonIfElseExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonIfElseExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonIfElseExpressionFactory? _ifElseExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _ifElseExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyIf,
                new JObject()
                {
                    { JsonSchemaPropertyCondition, null },
                    { JsonSchemaPropertyThen, null },
                    { JsonSchemaPropertyElse, null },
                }
            },
        };

        bool isMatch = _ifElseExpressionFactory!.Match(input);

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
                JsonSchemaPropertyIf,
                new JObject()
                {
                    { JsonSchemaPropertyCondition, null },
                    { JsonSchemaPropertyThen, null },
                }
            },
        };

        bool isMatch = _ifElseExpressionFactory!.Match(input);

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
                JsonSchemaPropertyIf,
                new JObject()
                {
                    { JsonSchemaPropertyCondition, null },
                    { JsonSchemaPropertyThen, null },
                }
            },
        };

        bool isMatch = _ifElseExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _ifElseExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateIfElseExpression()
    {
        // Setting up condition instruction mock
        JObject fakeConditionInstruction = new();
        Mock<IExpression<Task<bool>>> conditionExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<bool>>>(It.Is<JObject>(i => i == fakeConditionInstruction)))
            .Returns(conditionExpressionMock.Object);

        // Setting up then instruction mock
        JObject fakeThenInstruction = new();
        Mock<IExpression<Task>> thenExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task>>(It.Is<JObject>(i => i == fakeThenInstruction)))
            .Returns(thenExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertyIf,
                new JObject()
                {
                    { JsonSchemaPropertyCondition, fakeConditionInstruction },
                    { JsonSchemaPropertyThen, fakeThenInstruction },
                }
            },
        };

        IfElseExpression expression = _ifElseExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(2));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void Create_WhenElseExpressionSpecified_ShouldCreateIfElseExpression()
    {
        // Setting up condition instruction mock
        JObject fakeConditionInstruction = new();
        Mock<IExpression<Task<bool>>> conditionExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<bool>>>(It.Is<JObject>(i => i == fakeConditionInstruction)))
            .Returns(conditionExpressionMock.Object);

        // Setting up then instruction mock
        JObject fakeThenInstruction = new();
        Mock<IExpression<Task>> thenExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task>>(It.Is<JObject>(i => i == fakeThenInstruction)))
            .Returns(thenExpressionMock.Object);

        // Setting up else instruction mock
        JObject fakeElseInstruction = new();
        Mock<IExpression<Task>> elseExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task>>(It.Is<JObject>(i => i == fakeElseInstruction)))
            .Returns(elseExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertyIf,
                new JObject()
                {
                    { JsonSchemaPropertyCondition, fakeConditionInstruction },
                    { JsonSchemaPropertyThen, fakeThenInstruction },
                    { JsonSchemaPropertyElse, fakeElseInstruction },
                }
            },
        };

        IfElseExpression expression = _ifElseExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(3));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
