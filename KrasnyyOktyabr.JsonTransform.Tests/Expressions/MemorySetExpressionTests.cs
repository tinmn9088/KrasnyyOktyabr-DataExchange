using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class MemorySetExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemorySetExpression_WhenNameExpressionNull_ShouldThrowArgumentNullException()
    {
        new MemorySetExpression(null!, null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemorySetExpression_WhenValueExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<string>>> nameExpressionMock = new();

        new MemorySetExpression(nameExpressionMock.Object, null!);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldSaveValueToMemory()
    {
        // Setting up name expression
        Mock<IExpression<Task<string>>> nameExpressionMock = new();
        string name = "TestName";
        nameExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(name));

        // Setting up value expression
        Mock<IExpression<Task<object?>>> valueExpressionMock = new();
        object? expectedValue = "TestValue";
        valueExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((object?)expectedValue));

        // Setting up memory set expression
        MemorySetExpression memorySetExpression = new(nameExpressionMock.Object, valueExpressionMock.Object);

        Mock<IContext> contextMock = new();

        await memorySetExpression.InterpretAsync(contextMock.Object);

        nameExpressionMock.Verify(e => e.InterpretAsync(contextMock.Object, It.IsAny<CancellationToken>()), Times.Once());
        valueExpressionMock.Verify(e => e.InterpretAsync(contextMock.Object, It.IsAny<CancellationToken>()), Times.Once());
        contextMock.Verify(e => e.MemorySet(It.Is<string>(n => n == name), It.Is<object?>(v => v == expectedValue)), Times.Once());
    }

    [TestMethod]
    [ExpectedException(typeof(InterpretException))]
    public async Task InterpretAsync_WhenKeyNull_ShouldThrowInterpretException()
    {
        // Setting up null key expression
        Mock<IExpression<Task<string>>> keyExpressionMock = new();
        keyExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((string)null!));

        // Setting up value expression
        Mock<IExpression<Task<object?>>> valueExpressionMock = new();
        string expectedValue = "TestValue";
        valueExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((object?)expectedValue));

        // Setting up select expression
        MemorySetExpression memorySetExpression = new(keyExpressionMock.Object, valueExpressionMock.Object);

        await memorySetExpression.InterpretAsync(CreateEmptyExpressionContext());
    }
}
