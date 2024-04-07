using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonRegexGetGroupValueExpressionFactory;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonRegexGetGroupValueExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private JsonRegexGetGroupValueExpressionFactory? _getGroupValueExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _getGroupValueExpressionFactory = new(_abstractFactoryMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyRegexGetGroup,
                new JObject()
                {
                    { JsonSchemaPropertyRegex, null },
                    { JsonSchemaPropertyInput, null },
                    { JsonSchemaPropertyGroupNumber, null },
                }
            },
        };

        bool isMatch = _getGroupValueExpressionFactory!.Match(input);

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
                JsonSchemaPropertyRegexGetGroup,
                new JObject()
                {
                    { JsonSchemaPropertyRegex, null },
                    { JsonSchemaPropertyInput, null },
                }
            },
        };

        bool isMatch = _getGroupValueExpressionFactory!.Match(input);

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
                JsonSchemaPropertyRegexGetGroup,
                new JObject()
                {
                    { JsonSchemaPropertyRegex, null },
                    { JsonSchemaPropertyInput, null },
                }
            },
        };

        bool isMatch = _getGroupValueExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _getGroupValueExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateRegexGetGroupValueExpression()
    {
        // Setting up regex instruction mock
        JObject fakeRegexInstruction = new();
        Mock<IExpression<Task<string>>> regexExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<string>>>(fakeRegexInstruction))
            .Returns(regexExpressionMock.Object);

        // Setting up input instruction mock
        JObject fakeInputInstruction = new();
        Mock<IExpression<Task<string>>> inputExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<string>>>(fakeInputInstruction))
            .Returns(inputExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertyRegexGetGroup,
                new JObject()
                {
                    { JsonSchemaPropertyRegex, fakeRegexInstruction },
                    { JsonSchemaPropertyInput, fakeInputInstruction },
                }
            },
        };

        RegexGetGroupValueExpression expression = _getGroupValueExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(2));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void Create_WhenGroupNumberSpecified_ShouldCreateRegexGetGroupValueExpression()
    {
        // Setting up regex instruction mock
        JObject fakeRegexInstruction = new();
        Mock<IExpression<Task<string>>> regexExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<string>>>(fakeRegexInstruction))
            .Returns(regexExpressionMock.Object);

        // Setting up input instruction mock
        JObject fakeInputInstruction = new();
        Mock<IExpression<Task<string>>> inputExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<string>>>(fakeInputInstruction))
            .Returns(inputExpressionMock.Object);

        // Setting up group number instruction mock
        JObject fakeGroupNumberInstruction = new();
        Mock<IExpression<Task<int>>> groupNumberExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<int>>>(fakeGroupNumberInstruction))
            .Returns(groupNumberExpressionMock.Object);

        JObject input = new()
        {
            {
                JsonSchemaPropertyRegexGetGroup,
                new JObject()
                {
                    { JsonSchemaPropertyRegex, fakeRegexInstruction },
                    { JsonSchemaPropertyInput, fakeInputInstruction },
                    { JsonSchemaPropertyGroupNumber, fakeGroupNumberInstruction },
                }
            },
        };

        RegexGetGroupValueExpression expression = _getGroupValueExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(3));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
