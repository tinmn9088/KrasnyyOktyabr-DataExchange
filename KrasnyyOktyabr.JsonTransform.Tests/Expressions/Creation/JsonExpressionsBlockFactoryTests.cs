using Moq;
using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonExpressionsBlockFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonExpressionsBlockFactory? _expressionsBlockFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _expressionsBlockFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Create_ShouldCreateExpressionsBlock()
    {
        Mock<IExpression<Task<object?>>> expressionMock = new();
        _abstractFactoryMock!.Setup(f => f.Create<IExpression<Task<object?>>>(It.IsAny<JToken>())).Returns(expressionMock.Object);
        JObject fakeInstruction = new();
        JArray input = [fakeInstruction];

        ExpressionsBlock expression = _expressionsBlockFactory!.Create(input);

        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(fakeInstruction), Times.Once());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _expressionsBlockFactory!.Create(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Create_WhenInputNotArray_ShouldThrowArgumentException()
    {
        _expressionsBlockFactory!.Create(new JObject());
    }
}
