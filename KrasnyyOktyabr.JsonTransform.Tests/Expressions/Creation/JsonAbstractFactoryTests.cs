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

    private JsonConstExpressionFactory? _constExpressionFactory;

    private JsonExpressionsBlockFactory? _expressionsBlockFactory;

    private JsonAbstractExpressionFactory? _abstractFactory;

    [TestInitialize]
    public void Initialize()
    {
        string? testName = TestContext.TestName;

        if (testName != null && SkipTestInitialize(GetType(), testName))
        {
            return;
        }

        _abstractFactory = new();

        _constExpressionFactory = new();
        _expressionsBlockFactory = new(_abstractFactory);

        _abstractFactory.ExpressionFactories = [_constExpressionFactory, _expressionsBlockFactory];
    }

    [TestMethod]
    [SkipTestInitialize]
    public void Create_ShouldReturnExpressionWithoutWrapping()
    {
        // Setting up abstract expressions factory
        _abstractFactory = new();

        _constExpressionFactory = new();
        _expressionsBlockFactory = new(_abstractFactory);

        // Setting up expression factory mock
        Mock<IJsonExpressionFactory<IExpression<Task<object?>>>> factoryMock = new();
        Mock<IExpression<Task<object?>>> createdExpressionMock = new();
        factoryMock
            .Setup(mf => mf.Match(It.IsAny<JToken>()))
            .Returns(true);
        factoryMock
            .Setup(mf => mf.Create(It.IsAny<JToken>()))
            .Returns(createdExpressionMock.Object);

        _abstractFactory.ExpressionFactories = [factoryMock.Object];

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
    [SkipTestInitialize]
    public void Create_ShouldWrapIntExpressionInNumberCastExpression()
    {
        // Setting up abstract expressions factory
        _abstractFactory = new();

        _constExpressionFactory = new();
        _expressionsBlockFactory = new(_abstractFactory);

        // Setting up expression factory mock
        Mock<IJsonExpressionFactory<IExpression<Task<int>>>> factoryMock = new();
        Mock<IExpression<Task<int>>> createdIntExpressionMock = new();
        factoryMock
            .Setup(mf => mf.Match(It.IsAny<JToken>()))
            .Returns(true);
        factoryMock
            .Setup(mf => mf.Create(It.IsAny<JToken>()))
            .Returns(createdIntExpressionMock.Object);

        _abstractFactory.ExpressionFactories = [factoryMock.Object];

        JObject fakeInstruction = new();

        IExpression<Task<Number>> createdExpression = _abstractFactory.Create<IExpression<Task<Number>>>(fakeInstruction);

        Assert.IsInstanceOfType(createdExpression, typeof(NumberCastExpression));
    }

    [TestMethod]
    [SkipTestInitialize]
    public void Create_ShouldWrapDoubleExpressionInNumberCastExpression()
    {
        // Setting up abstract expressions factory
        _abstractFactory = new();

        _constExpressionFactory = new();
        _expressionsBlockFactory = new(_abstractFactory);

        // Setting up expression factory mock
        Mock<IJsonExpressionFactory<IExpression<Task<double>>>> factoryMock = new();
        Mock<IExpression<Task<double>>> createdDoubleExpressionMock = new();
        factoryMock
            .Setup(mf => mf.Match(It.IsAny<JToken>()))
            .Returns(true);
        factoryMock
            .Setup(mf => mf.Create(It.IsAny<JToken>()))
            .Returns(createdDoubleExpressionMock.Object);

        _abstractFactory.ExpressionFactories = [factoryMock.Object];

        JObject fakeInstruction = new();

        IExpression<Task<Number>> createdExpression = _abstractFactory.Create<IExpression<Task<Number>>>(fakeInstruction);

        Assert.IsInstanceOfType(createdExpression, typeof(NumberCastExpression));
    }

    [TestMethod]
    [SkipTestInitialize]
    public void Create_ShouldWrapInObjectCastExpression()
    {
        // Setting up abstract expressions factory
        _abstractFactory = new();

        _constExpressionFactory = new();
        _expressionsBlockFactory = new(_abstractFactory);

        // Setting up expression factory mock
        Mock<IJsonExpressionFactory<IExpression<Task>>> factoryMock = new();
        Mock<IExpression<Task>> createdExpressionMock = new();
        factoryMock
            .Setup(mf => mf.Match(It.IsAny<JToken>()))
            .Returns(true);
        factoryMock
            .Setup(mf => mf.Create(It.IsAny<JToken>()))
            .Returns(createdExpressionMock.Object);

        _abstractFactory.ExpressionFactories = [factoryMock.Object];

        JObject fakeInstruction = new();

        IExpression<Task<object?>> createdExpression = _abstractFactory.Create<IExpression<Task<object?>>>(fakeInstruction);

        Assert.IsInstanceOfType(createdExpression, typeof(ObjectCastExpression));
    }
}
