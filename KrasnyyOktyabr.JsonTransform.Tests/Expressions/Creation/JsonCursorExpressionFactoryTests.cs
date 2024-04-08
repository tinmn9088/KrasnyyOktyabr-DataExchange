using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonCursorExpressionFactory;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonCursorExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonCursorExpressionFactory? _cursorExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _cursorExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyCur,
                new JObject()
                {
                    { JsonSchemaPropertyName, null },
                }
            },
        };

        bool isMatch = _cursorExpressionFactory!.Match(input);

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
                JsonSchemaPropertyCur,
                new JObject()
            },
        };

        bool isMatch = _cursorExpressionFactory!.Match(input);

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
                JsonSchemaPropertyCur,
                new JObject()
            },
        };

        bool isMatch = _cursorExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _cursorExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateForeachCursorExpression()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyCur,
                new JObject()
            },
        };

        CursorExpression expression = _cursorExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock!.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void Create_WhenNameSpecified_ShouldCreateForeachCursorExpression()
    {
        // Setting up name instruction mock
        JObject fakeNameInstruction = new();
        Mock<IExpression<Task<string>>> nameExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<string>>>(fakeNameInstruction))
            .Returns(nameExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertyCur,
                new JObject()
                {
                    { JsonSchemaPropertyName, fakeNameInstruction },
                }
            },
        };

        CursorExpression expression = _cursorExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Once);
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
