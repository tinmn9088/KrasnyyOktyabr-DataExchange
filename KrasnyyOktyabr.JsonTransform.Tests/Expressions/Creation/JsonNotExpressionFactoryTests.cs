using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonNotExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonNotExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonNotExpressionFactory? _notExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _notExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyNot,
                new JObject()
                {
                    { JsonSchemaPropertyValue, null },
                }
            },
        };

        bool isMatch = _notExpressionFactory!.Match(input);

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
                JsonSchemaPropertyNot,
                new JObject()
                {
                    { JsonSchemaPropertyValue, null },
                }
            },
        };

        bool isMatch = _notExpressionFactory!.Match(input);

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
                JsonSchemaPropertyNot,
                new JObject()
                {
                    { JsonSchemaPropertyValue, null },
                }
            },
        };

        bool isMatch = _notExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _notExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateNotExpression()
    {
        // Setting up value instruction mock
        JObject fakeValueInstruction = new();
        Mock<IExpression<Task<bool>>> valueExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<bool>>>(fakeValueInstruction))
            .Returns(valueExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertyNot,
                new JObject()
                {
                    { JsonSchemaPropertyValue, fakeValueInstruction },
                }
            },
        };

        NotExpression expression = _notExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task<bool>>>(It.Is<JToken>(i => i == fakeValueInstruction)), Times.Once);
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
