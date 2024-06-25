using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonValueTableCreateExpressionFactory;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonValueTableCreateExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonValueTableCreateExpressionFactory? _valueTableCreateExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _valueTableCreateExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyCreateTable,
                new JObject()
                {
                    { JsonSchemaPropertyColumns, null },
                }
            },
        };

        bool isMatch = _valueTableCreateExpressionFactory!.Match(input);

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
                JsonSchemaPropertyCreateTable,
                new JObject()
                {
                    { JsonSchemaPropertyColumns, null },
                }
            },
        };

        bool isMatch = _valueTableCreateExpressionFactory!.Match(input);

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
                JsonSchemaPropertyCreateTable,
                new JObject()
                {
                    { JsonSchemaPropertyColumns, null },
                }
            },
        };

        bool isMatch = _valueTableCreateExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _valueTableCreateExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateCreateValueTableExpression()
    {
        // Setting up columns instruction mock
        JObject fakeColumnsStringInstruction = new();
        Mock<IExpression<Task<string>>> columnsStringExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<string>>>(fakeColumnsStringInstruction))
            .Returns(columnsStringExpressionMock.Object);

        JObject input = new()
        {
            { JsonSchemaPropertyComment, "TestComment" },
            {
                JsonSchemaPropertyCreateTable,
                new JObject()
                {
                    {
                        JsonSchemaPropertyColumns,
                        "Column1,Columns2"
                    },
                }
            },
        };

        ValueTableCreateExpression expression = _valueTableCreateExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Once());
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
