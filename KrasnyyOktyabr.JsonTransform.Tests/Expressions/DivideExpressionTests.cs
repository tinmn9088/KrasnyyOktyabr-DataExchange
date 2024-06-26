﻿using KrasnyyOktyabr.JsonTransform.Numerics;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class DivideExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void DivideExpression_WhenLeftExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<Number>>> rightExpressionMock = new();

        new DivideExpression(null!, rightExpressionMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void DivideExpression_WhenRightExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<Number>>> leftExpressionMock = new();

        new DivideExpression(leftExpressionMock.Object, null!);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldReturnResult()
    {
        Number leftExpressionResult = new(2);
        Number rightExpressionResult = new(3);

        // Setting up left expression
        Mock<IExpression<Task<Number>>> leftExpressionMock = new();
        leftExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(leftExpressionResult));

        // Setting up right expression
        Mock<IExpression<Task<Number>>> rightExpressionMock = new();
        rightExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(rightExpressionResult));

        DivideExpression divideExpression = new(leftExpressionMock.Object, rightExpressionMock.Object);

        Number expected = leftExpressionResult / rightExpressionResult;

        Number actual = await divideExpression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(expected, actual);
    }
}
