using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class MemoryGetExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemoryGetExpression_WhenNameExpressionNull_ShouldThrowArgumentNullException()
    {
        new MemoryGetExpression(null!);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldGetValueFromMemory()
    {
        // Setting up name expression
        Mock<IExpression<Task<string>>> nameExpressionMock = new();
        string name = "TestName";
        nameExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(name));

        // Setting up memory get expression
        MemoryGetExpression memoryGetExpression = new(nameExpressionMock.Object);

        string expected = "TestValue";
        Mock<IContext> contextMock = new();
        contextMock
            .Setup(c => c.MemoryGet(name))
            .Returns(expected);

        await memoryGetExpression.InterpretAsync(contextMock.Object);

        nameExpressionMock.Verify(e => e.InterpretAsync(contextMock.Object, It.IsAny<CancellationToken>()), Times.Once());
        contextMock.Verify(e => e.MemoryGet(It.Is<string>(n => n == name)), Times.Once());
    }

    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public async Task InterpretAsync_WhenKeyNull_ShouldThrowNullReferenceException()
    {
        // Setting up null key expression
        Mock<IExpression<Task<string>>> keyExpressionMock = new();
        keyExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((string)null!));

        // Setting up select expression
        MemoryGetExpression memoryGetExpression = new(keyExpressionMock.Object);

        await memoryGetExpression.InterpretAsync(CreateEmptyExpressionContext());
    }
}
