﻿using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonCastExpressionFactory;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonCastExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonCastExpressionFactory? _castExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _castExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyCast,
                new JObject()
                {
                    { JsonSchemaPropertyValue, null },
                    { JsonSchemaPropertyType, ReturnType.Int.ToString().ToLower() },
                }
            }
        };

        bool isMatch = _castExpressionFactory!.Match(input);

        Assert.IsTrue(isMatch);
    }

    [TestMethod]
    public void Match_WhenInputWithComment_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyComment,
                "Test comment"
            },
            {
                JsonSchemaPropertyCast,
                new JObject()
                {
                    { JsonSchemaPropertyValue, null },
                    { JsonSchemaPropertyType, ReturnType.String.ToString().ToLower() },
                }
            }
        };

        bool isMatch = _castExpressionFactory!.Match(input);

        Assert.IsTrue(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _castExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateIntCastExpression()
    {
        Mock<IExpression<Task>> expressionMock = new();
        _abstractFactoryMock!.Setup(f => f.Create<IExpression<Task>>(It.IsAny<JToken>())).Returns(expressionMock.Object);
        JObject valueInstruction = new();
        string type = ReturnType.Int.ToString().ToLower();
        JObject input = new()
        {
            {
                JsonSchemaPropertyCast,
                new JObject()
                {
                    { JsonSchemaPropertyValue, valueInstruction },
                    { JsonSchemaPropertyType, type },
                }
            }
        };

        IExpression<Task> expression = _castExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        Assert.IsInstanceOfType<IntCastExpression>(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(valueInstruction), Times.Once());
        _abstractFactoryMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void Create_ShouldCreateDoubleCastExpression()
    {
        Mock<IExpression<Task>> expressionMock = new();
        _abstractFactoryMock!.Setup(f => f.Create<IExpression<Task>>(It.IsAny<JToken>())).Returns(expressionMock.Object);
        JObject valueInstruction = new();
        string type = ReturnType.Float.ToString().ToLower();
        JObject input = new()
        {
            {
                JsonSchemaPropertyCast,
                new JObject()
                {
                    { JsonSchemaPropertyValue, valueInstruction },
                    { JsonSchemaPropertyType, type },
                }
            }
        };

        IExpression<Task> expression = _castExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        Assert.IsInstanceOfType<DoubleCastExpression>(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(valueInstruction), Times.Once());
        _abstractFactoryMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void Create_ShouldCreateBoolCastExpression()
    {
        Mock<IExpression<Task>> expressionMock = new();
        _abstractFactoryMock!.Setup(f => f.Create<IExpression<Task>>(It.IsAny<JToken>())).Returns(expressionMock.Object);
        JObject valueInstruction = new();
        string type = ReturnType.Bool.ToString().ToLower();
        JObject input = new()
        {
            {
                JsonSchemaPropertyCast,
                new JObject()
                {
                    { JsonSchemaPropertyValue, valueInstruction },
                    { JsonSchemaPropertyType, type },
                }
            }
        };

        IExpression<Task> expression = _castExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        Assert.IsInstanceOfType<BoolCastExpression>(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(valueInstruction), Times.Once());
        _abstractFactoryMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void Create_ShouldCreateStringCastExpression()
    {
        Mock<IExpression<Task>> expressionMock = new();
        _abstractFactoryMock!.Setup(f => f.Create<IExpression<Task>>(It.IsAny<JToken>())).Returns(expressionMock.Object);
        JObject valueInstruction = new();
        string type = ReturnType.String.ToString().ToLower();
        JObject input = new()
        {
            {
                JsonSchemaPropertyCast,
                new JObject()
                {
                    { JsonSchemaPropertyValue, valueInstruction },
                    { JsonSchemaPropertyType, type },
                }
            }
        };

        IExpression<Task> expression = _castExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        Assert.IsInstanceOfType<StringCastExpression>(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(valueInstruction), Times.Once());
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
