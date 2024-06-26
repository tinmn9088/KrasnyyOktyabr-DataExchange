﻿using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class StringCastExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StringCastExpression_WhenInnerExpressionNull_ShouldThrowArgumentNullException()
    {
        new StringCastExpression(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(InterpretException))]
    public async Task InterpretAsync_WhenInnerExpressionResultNull_ShouldThrowInterpretException()
    {
        // Setting up inner expression mock that returns null
        Mock<IExpression<Task<object?>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<object?>(null));

        StringCastExpression expression = new(innerExpressionMock.Object);

        await expression.InterpretAsync(CreateEmptyExpressionContext());
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldReturnString()
    {
        string expected = "TestString";

        // Setting up inner expression mock that returns integer
        Mock<IExpression<Task<string>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(expected));

        StringCastExpression expression = new(innerExpressionMock.Object);

        string actual = await expression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(expected, actual);
    }
}
