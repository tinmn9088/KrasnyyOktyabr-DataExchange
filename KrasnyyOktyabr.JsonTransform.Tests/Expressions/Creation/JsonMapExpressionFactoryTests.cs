using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonMapExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonMapFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonMapExpressionFactory? _mapExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _mapExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _mapExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateMapExpression()
    {
        // Setting up instruction mock
        JObject fakeInstruction = new();
        Mock<IExpression<Task<object?>>> expressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<object?>>>(fakeInstruction))
            .Returns(expressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertyMap,
                new JObject()
                {
                    { "Key", fakeInstruction },
                }
            },
        };

        MapExpression expression = _mapExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Once);
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
