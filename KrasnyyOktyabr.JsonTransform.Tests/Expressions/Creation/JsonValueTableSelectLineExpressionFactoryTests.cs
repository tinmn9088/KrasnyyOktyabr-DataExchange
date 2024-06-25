using KrasnyyOktyabr.JsonTransform.Structures;
using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonValueTableSelectLineExpressionFactory;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonValueTableSelectLineExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonValueTableSelectLineExpressionFactory? _valueTableSelectLineExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _valueTableSelectLineExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertySelectLine,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                    { JsonSchemaPropertyIndex, null },
                }
            },
        };

        bool isMatch = _valueTableSelectLineExpressionFactory!.Match(input);

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
                JsonSchemaPropertySelectLine,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                    { JsonSchemaPropertyIndex, null },
                }
            },
        };

        bool isMatch = _valueTableSelectLineExpressionFactory!.Match(input);

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
                JsonSchemaPropertySelectLine,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                    { JsonSchemaPropertyIndex, null },
                }
            },
        };

        bool isMatch = _valueTableSelectLineExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _valueTableSelectLineExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateSelectLineInValueTableExpression()
    {
        // Setting up value table instruction mock
        Mock<IExpression<Task<IValueTable>>> addLineToValueTableExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<IValueTable>>>(It.IsAny<JToken>()))
            .Returns(addLineToValueTableExpressionMock.Object);

        // Setting up index instruction mock
        Mock<IExpression<Task<int>>> selectLineInValueTableExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<int>>>(It.IsAny<JToken>()))
            .Returns(selectLineInValueTableExpressionMock.Object);

        JObject input = new()
        {
            { JsonSchemaPropertyComment, "TestComment" },
            {
                JsonSchemaPropertySelectLine,
                new JObject()
                {
                    { JsonSchemaPropertyTable, new JObject() },
                    { JsonSchemaPropertyIndex, new JObject() },
                }
            },
        };

        ValueTableSelectLineExpression expression = _valueTableSelectLineExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(2));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
