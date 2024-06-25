using KrasnyyOktyabr.JsonTransform.Structures;
using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonValueTableGetValueExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonValueTableGetValueExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonValueTableGetValueExpressionFactory? _valueTableGetValueExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _valueTableGetValueExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyGetValue,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                    { JsonSchemaPropertyColumn, null },
                }
            },
        };

        bool isMatch = _valueTableGetValueExpressionFactory!.Match(input);

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
                JsonSchemaPropertyGetValue,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                    { JsonSchemaPropertyColumn, null },
                }
            },
        };

        bool isMatch = _valueTableGetValueExpressionFactory!.Match(input);

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
                JsonSchemaPropertyGetValue,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                    { JsonSchemaPropertyColumn, null },
                }
            },
        };

        bool isMatch = _valueTableGetValueExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _valueTableGetValueExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateSelectLineInValueTableExpression()
    {
        // Setting up value table instruction mock
        Mock<IExpression<Task<IValueTable>>> addLineToValueTableExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<IValueTable>>>(It.IsAny<JToken>()))
            .Returns(addLineToValueTableExpressionMock.Object);

        // Setting up column instruction mock
        Mock<IExpression<Task<string>>> columnExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<string>>>(It.IsAny<JToken>()))
            .Returns(columnExpressionMock.Object);

        JObject input = new()
        {
            { JsonSchemaPropertyComment, "TestComment" },
            {
                JsonSchemaPropertyGetValue,
                new JObject()
                {
                    { JsonSchemaPropertyTable, new JObject() },
                    { JsonSchemaPropertyColumn, new JObject() },
                }
            },
        };

        ValueTableGetValueExpression expression = _valueTableGetValueExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(2));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
