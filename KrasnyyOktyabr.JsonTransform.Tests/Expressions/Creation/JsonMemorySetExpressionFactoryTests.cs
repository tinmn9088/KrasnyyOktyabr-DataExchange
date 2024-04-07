using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonMemorySetExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonMemorySetExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonMemorySetExpressionFactory? _memorySetExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _memorySetExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyMSet,
                new JObject()
                {
                    { JsonSchemaPropertyName, null },
                    { JsonSchemaPropertyValue, null },
                }
            },
        };

        bool isMatch = _memorySetExpressionFactory!.Match(input);

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
                JsonSchemaPropertyMSet,
                new JObject()
                {
                    { JsonSchemaPropertyName, null },
                    { JsonSchemaPropertyValue, null },
                }
            },
        };

        bool isMatch = _memorySetExpressionFactory!.Match(input);

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
                JsonSchemaPropertyMSet,
                new JObject()
                {
                    { JsonSchemaPropertyName, null },
                    { JsonSchemaPropertyValue, null },
                }
            },
        };

        bool isMatch = _memorySetExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _memorySetExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateMemorySetExpression()
    {
        // Setting up name instruction mock
        JObject fakeNameInstruction = new();
        Mock<IExpression<Task<string>>> nameExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<string>>>(fakeNameInstruction))
            .Returns(nameExpressionMock.Object);

        // Setting up value instruction mock
        JObject fakeValueInstruction = new();
        Mock<IExpression<Task<object?>>> valueExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<object?>>>(fakeValueInstruction))
            .Returns(valueExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertyMSet,
                new JObject()
                {
                    { JsonSchemaPropertyName, fakeNameInstruction },
                    { JsonSchemaPropertyValue, fakeValueInstruction },
                }
            },
        };

        MemorySetExpression expression = _memorySetExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(2));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
