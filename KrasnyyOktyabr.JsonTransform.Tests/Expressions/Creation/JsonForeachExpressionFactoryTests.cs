using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonForeachExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonForeachExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonForeachExpressionFactory? _foreachExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _foreachExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyForeach,
                new JObject()
                {
                    { JsonSchemaPropertyName, "TestName" },
                    { JsonSchemaPropertyItems, null },
                    { JsonSchemaPropertyInstructions, null },
                }
            },
        };

        bool isMatch = _foreachExpressionFactory!.Match(input);

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
                JsonSchemaPropertyForeach,
                new JObject()
                {
                    { JsonSchemaPropertyItems, null },
                    { JsonSchemaPropertyInstructions, null },
                }
            },
        };

        bool isMatch = _foreachExpressionFactory!.Match(input);

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
                JsonSchemaPropertyForeach,
                new JObject()
                {
                    { JsonSchemaPropertyItems, null },
                    { JsonSchemaPropertyInstructions, null },
                }
            },
        };

        bool isMatch = _foreachExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _foreachExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateForeachExpression()
    {
        // Setting up items instruction mock
        JObject fakeItemsInstruction = new();
        Mock<IExpression<Task<object?[]>>> itemsExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<object?[]>>>(It.Is<JObject>(i => i == fakeItemsInstruction)))
            .Returns(itemsExpressionMock.Object);

        // Setting up instructions mock
        JObject fakeInstructions = new();
        Mock<IExpression<Task>> innerExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task>>(It.Is<JObject>(i => i == fakeInstructions)))
            .Returns(innerExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertyForeach,
                new JObject()
                {
                    { JsonSchemaPropertyName, "TestName" },
                    { JsonSchemaPropertyItems, fakeItemsInstruction },
                    { JsonSchemaPropertyInstructions, fakeInstructions },
                }
            },
        };

        ForeachExpression expression = _foreachExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(2));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
