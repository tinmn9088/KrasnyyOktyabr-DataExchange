using KrasnyyOktyabr.JsonTransform.Numerics;
using KrasnyyOktyabr.JsonTransform.Tests;
using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonConstExpressionFactory;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonAbstractFactoryTests
{
    public required TestContext TestContext
    {
        get; set;
    }

    private JsonAbstractExpressionFactory? _abstractFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactory = new();
    }

    [TestMethod]
    public void Create_ShouldReturnExpressionWithoutWrapping()
    {
        // Setting up expression factory mock
        Mock<IJsonExpressionFactory<IExpression<Task<object?>>>> factoryMock = new();
        Mock<IExpression<Task<object?>>> createdExpressionMock = new();
        factoryMock
            .Setup(mf => mf.Match(It.IsAny<JToken>()))
            .Returns(true);
        factoryMock
            .Setup(mf => mf.Create(It.IsAny<JToken>()))
            .Returns(createdExpressionMock.Object);

        _abstractFactory!.ExpressionFactories = [factoryMock.Object];

        JObject fakeInstruction = new();

        IExpression<Task<object?>> createdExpression = _abstractFactory.Create<IExpression<Task<object?>>>(fakeInstruction);

        Assert.AreSame(createdExpression, createdExpressionMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInstructionNull_ShouldThrowArgumentNullException()
    {
        _abstractFactory!.Create<IExpression<Task>>(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonAbstractExpressionFactory.UnknownInstructionException))]
    public void Create_WhenInstructionUnknown_ShouldThrowUnknownExpressionException()
    {
        JObject invalidInstruction = new()
        {
            { "UnknownExpression", "Value" },
        };

        _abstractFactory!.Create<IExpression<Task>>(invalidInstruction);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonAbstractExpressionFactory.InvalidExpressionReturnTypeException))]
    public void Create_WhenExpressionReturnTypeInvalid_ShouldThrowInvalidExpressionReturnTypeException()
    {
        // Setting up abstract expression factory
        JsonConstExpressionFactory constExpressionFactory = new();
        _abstractFactory!.ExpressionFactories = [constExpressionFactory];

        // Setting up instruction
        JToken testValue = "TestValue";
        JObject objectInstruction = new()
        {
            { JsonSchemaPropertyConst, testValue },
        };

        // Try to get expression that returns string
        _abstractFactory!.Create<IExpression<Task<string>>>(objectInstruction);
    }

    [TestMethod]
    public async Task Create_WhenConstInstruction_ShouldReturnConstExpression()
    {
        // Setting up abstract expression factory
        JsonConstExpressionFactory constExpressionFactory = new();
        _abstractFactory!.ExpressionFactories = [constExpressionFactory];

        // Setting up instruction
        JToken testValue = "TestValue";
        JObject instruction = new()
        {
            { JsonSchemaPropertyConst, testValue },
        };

        IExpression<Task> expression = _abstractFactory!.Create<IExpression<Task>>(instruction);

        Assert.IsNotNull(expression);
        Assert.IsInstanceOfType(expression, typeof(ConstExpression));

        // Run expression
        ConstExpression constExpression = (ConstExpression)expression;
        object? result = await constExpression.InterpretAsync(GetEmptyExpressionContext());

        Assert.AreEqual(testValue, result);
    }

    [TestMethod]
    public void Create_ShouldWrapIntExpressionInNumberCastExpression()
    {
        // Setting up expression factory mock
        Mock<IJsonExpressionFactory<IExpression<Task<int>>>> factoryMock = new();
        Mock<IExpression<Task<int>>> createdIntExpressionMock = new();
        factoryMock
            .Setup(mf => mf.Match(It.IsAny<JToken>()))
            .Returns(true);
        factoryMock
            .Setup(mf => mf.Create(It.IsAny<JToken>()))
            .Returns(createdIntExpressionMock.Object);

        _abstractFactory!.ExpressionFactories = [factoryMock.Object];

        JObject fakeInstruction = new();

        IExpression<Task<Number>> createdExpression = _abstractFactory.Create<IExpression<Task<Number>>>(fakeInstruction);

        Assert.IsInstanceOfType(createdExpression, typeof(NumberCastExpression));
    }

    [TestMethod]
    public void Create_ShouldWrapDoubleExpressionInNumberCastExpression()
    {
        // Setting up expression factory mock
        Mock<IJsonExpressionFactory<IExpression<Task<double>>>> factoryMock = new();
        Mock<IExpression<Task<double>>> createdDoubleExpressionMock = new();
        factoryMock
            .Setup(mf => mf.Match(It.IsAny<JToken>()))
            .Returns(true);
        factoryMock
            .Setup(mf => mf.Create(It.IsAny<JToken>()))
            .Returns(createdDoubleExpressionMock.Object);

        _abstractFactory!.ExpressionFactories = [factoryMock.Object];

        JObject fakeInstruction = new();

        IExpression<Task<Number>> createdExpression = _abstractFactory.Create<IExpression<Task<Number>>>(fakeInstruction);

        Assert.IsInstanceOfType(createdExpression, typeof(NumberCastExpression));
    }

    [TestMethod]
    public void Create_ShouldWrapInObjectCastExpression()
    {
        // Setting up expression factory mock
        Mock<IJsonExpressionFactory<IExpression<Task>>> factoryMock = new();
        Mock<IExpression<Task>> createdExpressionMock = new();
        factoryMock
            .Setup(mf => mf.Match(It.IsAny<JToken>()))
            .Returns(true);
        factoryMock
            .Setup(mf => mf.Create(It.IsAny<JToken>()))
            .Returns(createdExpressionMock.Object);

        _abstractFactory!.ExpressionFactories = [factoryMock.Object];

        JObject fakeInstruction = new();

        IExpression<Task<object?>> createdExpression = _abstractFactory.Create<IExpression<Task<object?>>>(fakeInstruction);

        Assert.IsInstanceOfType(createdExpression, typeof(ObjectCastExpression));
    }

    /// <summary>
    /// Solve ((5 - 1) + 3) * 6.
    /// </summary>
    [TestMethod]
    public async Task Create_ShouldCreateComplexExpression()
    {
        JsonConstExpressionFactory constExpressionFactory = new();
        JsonCastExpressionFactory castExpressionFactory = new(_abstractFactory!);
        JsonSumExpressionFactory sumExpressionFactory = new(_abstractFactory!);
        JsonSubstractExpressionFactory substractExpressionFactory = new(_abstractFactory!);
        JsonMultiplyExpressionFactory multiplyExpressionFactory = new(_abstractFactory!);

        _abstractFactory!.ExpressionFactories =
        [
            constExpressionFactory,
            castExpressionFactory,
            sumExpressionFactory,
            substractExpressionFactory,
            multiplyExpressionFactory
        ];

        JObject input = JObject.Parse(GetTestInputInstruction(GetType(), TestContext.TestName ?? throw new NullReferenceException()));

        MultiplyExpression expression = _abstractFactory.Create<MultiplyExpression>(input);

        Number expected = new((5 - 1 + 3) * 6);

        Number actual = await expression.InterpretAsync(GetEmptyExpressionContext());

        Assert.AreEqual(expected, actual);
    }
}
