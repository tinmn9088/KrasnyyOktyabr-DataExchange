using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonSelectExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonSelectExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonSelectExpressionFactory? _selectExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _selectExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertySelect,
                new JObject()
                {
                    { JsonSchemaPropertyPath, null },
                    { JsonSchemaPropertyOptional, null },
                }
            },
        };

        bool isMatch = _selectExpressionFactory!.Match(input);

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
                JsonSchemaPropertySelect,
                new JObject()
                {
                    { JsonSchemaPropertyPath, null },
                }
            },
        };

        bool isMatch = _selectExpressionFactory!.Match(input);

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
                JsonSchemaPropertySelect,
                new JObject()
                {
                    { JsonSchemaPropertyPath, null },
                }
            },
        };

        bool isMatch = _selectExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _selectExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateSelectExpression()
    {
        // Setting up path instruction mock
        JObject fakePathInstruction = new();
        Mock<IExpression<Task<string>>> pathExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<string>>>(fakePathInstruction))
            .Returns(pathExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertySelect,
                new JObject()
                {
                    { JsonSchemaPropertyPath, fakePathInstruction },
                }
            },
        };

        SelectExpression expression = _selectExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Once);
        _abstractFactoryMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void Create_WhenIsOptionalSpecified_ShouldCreateSelectExpression()
    {
        // Setting up path instruction mock
        JObject fakePathInstruction = new();
        Mock<IExpression<Task<string>>> pathExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<string>>>(fakePathInstruction))
            .Returns(pathExpressionMock.Object);

        // Setting up is optional instruction mock
        JObject fakeIsOptionalInstruction = new();
        Mock<IExpression<Task<bool>>> isOptionakExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<bool>>>(fakeIsOptionalInstruction))
            .Returns(isOptionakExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertySelect,
                new JObject()
                {
                    { JsonSchemaPropertyPath, fakePathInstruction },
                    { JsonSchemaPropertyOptional, fakeIsOptionalInstruction },
                }
            },
        };

        SelectExpression expression = _selectExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(2));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
