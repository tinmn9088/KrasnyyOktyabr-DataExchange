using Moq;
using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonArrayExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonArrayExpressionFactory? _arrayExpressionsFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _arrayExpressionsFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Create_ShouldCreateArrayExpression()
    {
        Mock<IExpression<Task<object?>>> expressionMock = new();
        _abstractFactoryMock!.Setup(f => f.Create<IExpression<Task<object?>>>(It.IsAny<JToken>())).Returns(expressionMock.Object);
        JObject fakeInstruction = new();
        JArray input = [fakeInstruction];

        ArrayExpression expression = _arrayExpressionsFactory!.Create(input);

        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(fakeInstruction), Times.Once());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _arrayExpressionsFactory!.Create(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Create_WhenInputNotArray_ShouldThrowArgumentException()
    {
        _arrayExpressionsFactory!.Create(new JObject());
    }
}
