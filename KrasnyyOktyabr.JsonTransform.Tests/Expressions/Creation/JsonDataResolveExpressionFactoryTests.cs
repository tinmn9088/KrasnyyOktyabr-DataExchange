using KrasnyyOktyabr.JsonTransform.Expressions.DataResolve;
using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonDataResolveExpressionFactory;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation.Tests;

[TestClass]
public class JsonDataResolverExpressionFactoryTests
{
    private Mock<IJsonAbstractExpressionFactory>? _abstractFactoryMock;

    private Mock<IDataResolveService>? _dataResolveServiceMock;

    private JsonDataResolveExpressionFactory? _dataResolveExpressionFactory;

    [TestInitialize]
    public void Initialize()
    {
        _abstractFactoryMock = new();
        _dataResolveServiceMock = new();
        _dataResolveExpressionFactory = new(_abstractFactoryMock.Object, _dataResolveServiceMock.Object);
    }

    [TestMethod]
    public void Match_ShouldReturnTrue()
    {
        JObject input = new()
        {
            {
                JsonSchemaPropertyResolve,
                new JObject()
                {
                    { JsonSchemaPropertyResolver, null },
                    { JsonSchemaPropertyParams, null },
                }
            },
        };

        bool isMatch = _dataResolveExpressionFactory!.Match(input);

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
                JsonSchemaPropertyResolve,
                new JObject()
                {
                    { JsonSchemaPropertyResolver, null },
                    { JsonSchemaPropertyParams, null },
                }
            },
        };

        bool isMatch = _dataResolveExpressionFactory!.Match(input);

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
                JsonSchemaPropertyResolve,
                new JObject()
                {
                    { JsonSchemaPropertyResolver, null },
                    { JsonSchemaPropertyParams, null },
                }
            },
        };

        bool isMatch = _dataResolveExpressionFactory!.Match(input);

        Assert.IsFalse(isMatch);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_WhenInputNull_ShouldThrowArgumentNullException()
    {
        _dataResolveExpressionFactory!.Create(null!);
    }

    [TestMethod]
    public void Create_ShouldCreateDataResolveExpression()
    {
        // Setting up resolver instruction mock
        JObject fakeResovlerInstruction = new();
        Mock<IExpression<Task<string>>> resolverExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<string>>>(fakeResovlerInstruction))
            .Returns(resolverExpressionMock.Object);

        // Setting up params instruction mock
        JObject fakeParamsInstruction = new();
        Mock<IExpression<Task<Dictionary<string, object?>>>> paramsExpressionMock = new();
        _abstractFactoryMock!
            .Setup(f => f.Create<IExpression<Task<Dictionary<string, object?>>>>(fakeParamsInstruction))
            .Returns(paramsExpressionMock.Object);

        JObject input = new()
        {
            { JsonSchemaPropertyComment, "TestComment" },
            {
                JsonSchemaPropertyResolve,
                new JObject()
                {
                    { JsonSchemaPropertyResolver, fakeResovlerInstruction },
                    { JsonSchemaPropertyParams, fakeParamsInstruction },
                }
            },
        };

        DataResolveExpression expression = _dataResolveExpressionFactory!.Create(input);

        Assert.IsNotNull(expression);
        _abstractFactoryMock.Verify(f => f.Create<IExpression<Task>>(It.IsAny<JToken>()), Times.Exactly(2));
        _abstractFactoryMock.VerifyNoOtherCalls();
    }
}
