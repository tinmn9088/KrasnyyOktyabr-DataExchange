using KrasnyyOktyabr.JsonTransform.Structures;
using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonValueTableCollapseExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonValueTableCollapseExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonValueTableCollapseExpressionFactory? _valueTableCollapseExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _valueTableCollapseExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyCollapse,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                    { JsonSchemaPropertyColumnsToGroup, null },
                    { JsonSchemaPropertyColumnsToSum, null },
                }
            },
        };

        bool isMatch = _valueTableCollapseExpressionFactory!.Match(input);

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
                JsonSchemaPropertyCollapse,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                    { JsonSchemaPropertyColumnsToGroup, null },
                    { JsonSchemaPropertyColumnsToSum, null },
                }
            },
        };

        bool isMatch = _valueTableCollapseExpressionFactory!.Match(input);

        Assert.IsTrue(isMatch);
    }

    [TestMethod]
    public void Match_WhenInputWithoutColumnsToSum_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyComment, "TestComment"
            },
            {
                JsonSchemaPropertyCollapse,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                    { JsonSchemaPropertyColumnsToGroup, null },
                }
            },
        };

        bool isMatch = _valueTableCollapseExpressionFactory!.Match(input);

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
                JsonSchemaPropertyCollapse,
                new JObject()
                {
                    { JsonSchemaPropertyTable, null },
                    { JsonSchemaPropertyColumnsToGroup, null },
                    { JsonSchemaPropertyColumnsToSum, null },
                }
            },
        };

        bool isMatch = _valueTableCollapseExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _valueTableCollapseExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateValueTableCollapseExpression()
    {
        // Setting up value table instruction mock
        Mock<IExpression<Task<IValueTable>>> addLineToValueTableExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<IValueTable>>>(It.IsAny<JToken>()))
            .Returns(addLineToValueTableExpressionMock.Object);

        // Setting up column instruction mock
        Mock<IExpression<Task<string>>> columnsToGroupStringExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<string>>>(It.IsAny<JToken>()))
            .Returns(columnsToGroupStringExpressionMock.Object);

        // Setting up value instruction mock
        Mock<IExpression<Task<string>>> columnsToSumStringExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<string>>>(It.IsAny<JToken>()))
            .Returns(columnsToSumStringExpressionMock.Object);

        JObject input = new()
        {
            { JsonSchemaPropertyComment, "TestComment" },
            {
                JsonSchemaPropertyCollapse,
                new JObject()
                {
                    { JsonSchemaPropertyTable, new JObject() },
                    { JsonSchemaPropertyColumnsToGroup, new JObject() },
                    { JsonSchemaPropertyColumnsToSum, new JObject() },
                }
            },
        };

        ValueTableCollapseExpression expression = _valueTableCollapseExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(3));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
