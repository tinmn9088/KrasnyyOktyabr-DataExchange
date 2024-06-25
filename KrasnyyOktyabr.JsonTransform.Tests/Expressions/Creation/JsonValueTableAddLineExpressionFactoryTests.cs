using KrasnyyOktyabr.JsonTransform.Structures;
using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonValueTableAddLineExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonValueTableAddLineExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonValueTableAddLineExpressionFactory? _valueTableAddLineExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _valueTableAddLineExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyAddLine,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                }
            },
        };

        bool isMatch = _valueTableAddLineExpressionFactory!.Match(input);

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
                JsonSchemaPropertyAddLine,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                }
            },
        };

        bool isMatch = _valueTableAddLineExpressionFactory!.Match(input);

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
                JsonSchemaPropertyAddLine,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                }
            },
        };

        bool isMatch = _valueTableAddLineExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _valueTableAddLineExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateAddLineToValueTableExpression()
    {
        // Setting up value table instruction mock
        Mock<IExpression<Task<IValueTable>>> addLineToValueTableExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<IValueTable>>>(It.IsAny<JToken>()))
            .Returns(addLineToValueTableExpressionMock.Object);

        JObject input = new()
        {
            { JsonSchemaPropertyComment, "TestComment" },
            {
                JsonSchemaPropertyAddLine,
                new JObject()
                {
                    {
                        JsonSchemaPropertyTable,
                        new JObject()
                    },
                }
            },
        };

        ValueTableAddLineExpression expression = _valueTableAddLineExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Once());
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
