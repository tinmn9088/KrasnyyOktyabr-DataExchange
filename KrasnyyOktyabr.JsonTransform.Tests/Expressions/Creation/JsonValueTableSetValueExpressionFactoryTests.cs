using KrasnyyOktyabr.JsonTransform.Structures;
using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonValueTableSetValueExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;
[TestClass]
public class JsonValueTableSetValueExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonValueTableSetValueExpressionFactory? _valueTableSetValueExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _valueTableSetValueExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertySetValue,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                    { JsonSchemaPropertyColumn, null },
                    { JsonSchemaPropertyValue, null },
                }
            },
        };

        bool isMatch = _valueTableSetValueExpressionFactory!.Match(input);

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
                JsonSchemaPropertySetValue,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                    { JsonSchemaPropertyColumn, null },
                    { JsonSchemaPropertyValue, null },
                }
            },
        };

        bool isMatch = _valueTableSetValueExpressionFactory!.Match(input);

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
                JsonSchemaPropertySetValue,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                    { JsonSchemaPropertyColumn, null },
                    { JsonSchemaPropertyValue, null },
                }
            },
        };

        bool isMatch = _valueTableSetValueExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _valueTableSetValueExpressionFactory!.Create(null!);
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

        // Setting up value instruction mock
        Mock<IExpression<Task<object?>>> valueExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<object?>>>(It.IsAny<JToken>()))
            .Returns(valueExpressionMock.Object);

        JObject input = new()
        {
            { JsonSchemaPropertyComment, "TestComment" },
            {
                JsonSchemaPropertySetValue,
                new JObject()
                {
                    { JsonSchemaPropertyTable, new JObject() },
                    { JsonSchemaPropertyColumn, new JObject() },
                    { JsonSchemaPropertyValue, new JObject() },
                }
            },
        };

        ValueTableSetValueExpression expression = _valueTableSetValueExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(3));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
