using KrasnyyOktyabr.JsonTransform.Structures;
using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonValueTableAddColumnExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonValueTableAddColumnExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonValueTableAddColumnExpressionFactory? _valueTableAddColumnExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _valueTableAddColumnExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyAddColumn,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                    { JsonSchemaPropertyColumn, null },
                }
            },
        };

        bool isMatch = _valueTableAddColumnExpressionFactory!.Match(input);

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
                JsonSchemaPropertyAddColumn,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                    { JsonSchemaPropertyColumn, null },
                }
            },
        };

        bool isMatch = _valueTableAddColumnExpressionFactory!.Match(input);

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
                JsonSchemaPropertyAddColumn,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                    { JsonSchemaPropertyColumn, null },
                }
            },
        };

        bool isMatch = _valueTableAddColumnExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _valueTableAddColumnExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateValueTableAddColumnExpression()
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
                JsonSchemaPropertyAddColumn,
                new JObject()
                {
                    { JsonSchemaPropertyTable, new JObject() },
                    { JsonSchemaPropertyColumn, new JObject() },
                }
            },
        };

        ValueTableAddColumnExpression expression = _valueTableAddColumnExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(2));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
