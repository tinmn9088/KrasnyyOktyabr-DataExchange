using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonMemoryGetExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonMemoryGetExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonMemoryGetExpressionFactory? _memoryGetExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _memoryGetExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyMGet,
                new JObject()
                {
                    { JsonSchemaPropertyName, null },
                }
            },
        };

        bool isMatch = _memoryGetExpressionFactory!.Match(input);

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
                JsonSchemaPropertyMGet,
                new JObject()
                {
                    { JsonSchemaPropertyName, null },
                }
            },
        };

        bool isMatch = _memoryGetExpressionFactory!.Match(input);

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
                JsonSchemaPropertyMGet,
                new JObject()
                {
                    { JsonSchemaPropertyName, null },
                }
            },
        };

        bool isMatch = _memoryGetExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _memoryGetExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateMemoryGetExpression()
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
                JsonSchemaPropertyMGet,
                new JObject()
                {
                    { JsonSchemaPropertyName, fakeNameInstruction },
                }
            },
        };

        MemoryGetExpression expression = _memoryGetExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Once);
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
