using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonAddExpressionFactory;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonAddExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonAddExpressionFactory? _addExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _addExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyAdd,
                new JObject()
                {
                    { JsonSchemaPropertyKey, null },
                    { JsonSchemaPropertyValue, null },
                    { JsonSchemaPropertyIndex, null },
                }
            },
        };

        bool isMatch = _addExpressionFactory!.Match(input);

        Assert.IsTrue(isMatch);
    }

    [TestMethod]
    public void Match_WhenInputWithComment_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyComment, "TestComment"
            },
            {
                JsonSchemaPropertyAdd,
                new JObject()
                {
                    { JsonSchemaPropertyKey, null },
                    { JsonSchemaPropertyValue, null },
                }
            },
        };

        bool isMatch = _addExpressionFactory!.Match(input);

        Assert.IsTrue(isMatch);
    }

    [TestMethod]
    public void Match_WhenInputWithAdditionalProperties_ShouldReturnFalse()
    {
        JObject input = new()
        {
            {
                "AdditionalProperty", null
            },
            {
                JsonSchemaPropertyComment, "TestComment"
            },
            {
                JsonSchemaPropertyAdd,
                new JObject()
                {
                    { JsonSchemaPropertyKey, null },
                    { JsonSchemaPropertyValue, null },
                }
            },
        };

        bool isMatch = _addExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _addExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateAddExpression()
    {
        // Setting up key instruction mock
        JObject fakeKeyInstruction = new();
        Mock<IExpression<Task<string>>> keyExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<string>>>(fakeKeyInstruction))
            .Returns(keyExpressionMock.Object);

        // Setting up value instruction mock
        JObject fakeValueInstruction = new();
        Mock<IExpression<Task<object?>>> valueExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<object?>>>(fakeValueInstruction))
            .Returns(valueExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertyAdd,
                new JObject()
                {
                    { JsonSchemaPropertyKey, fakeKeyInstruction },
                    { JsonSchemaPropertyValue, fakeValueInstruction },
                }
            },
        };

        AddExpression expression = _addExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(2));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void Create_WhenIndexSpecified_ShouldCreateAddExpression()
    {
        // Setting up key instruction mock
        JObject fakeKeyInstruction = new();
        Mock<IExpression<Task<string>>> keyExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<string>>>(fakeKeyInstruction))
            .Returns(keyExpressionMock.Object);

        // Setting up value instruction mock
        JObject fakeValueInstruction = new();
        Mock<IExpression<Task<object?>>> valueExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<object?>>>(fakeValueInstruction))
            .Returns(valueExpressionMock.Object);

        // Setting up index instruction mock
        JObject fakeIndexInstruction = new();
        Mock<IExpression<Task<int>>> indexExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<int>>>(fakeIndexInstruction))
            .Returns(indexExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertyAdd,
                new JObject()
                {
                    { JsonSchemaPropertyKey, fakeKeyInstruction },
                    { JsonSchemaPropertyValue, fakeValueInstruction },
                    { JsonSchemaPropertyIndex, fakeIndexInstruction },
                }
            },
        };

        AddExpression expression = _addExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(3));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
