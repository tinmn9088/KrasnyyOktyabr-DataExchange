using System.Runtime.Versioning;
using Confluent.Kafka;
using KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;
using KrasnyyOktyabr.ComV77Application;
using KrasnyyOktyabr.ComV77Application.Contracts.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using static KrasnyyOktyabr.Application.Services.IJsonService;
using static KrasnyyOktyabr.Application.Services.IV77ApplicationLogService;
using static KrasnyyOktyabr.Application.Services.Kafka.V77ApplicationProducerService;

namespace KrasnyyOktyabr.Application.Services.Kafka.Tests;

[SupportedOSPlatform("windows")]
[TestClass]
public class V77ApplicationProducerServiceTests
{
    private static readonly Mock<IConfiguration> s_configurationMock = new();

    private static readonly Mock<ILogger<V77ApplicationProducerService>> s_loggerMock = new();

    private static readonly Mock<ILoggerFactory> s_loggerFactoryMock = new();

    private static readonly Mock<IOffsetService> s_offsetServiceMock = new();

    private static readonly Mock<IV77ApplicationLogService> s_logServiceMock = new();

    private static readonly Mock<IComV77ApplicationConnectionFactory> s_connectionFactoryMock = new();

    private static readonly Mock<IJsonService> s_jsonServiceMock = new();

    private static readonly Mock<IKafkaService> s_kafkaServiceMock = new();

    private static readonly V77ApplicationProducerService s_service = new(
        s_configurationMock.Object,
        s_loggerMock.Object,
        s_loggerFactoryMock.Object,
        s_offsetServiceMock.Object,
        s_logServiceMock.Object,
        s_connectionFactoryMock.Object,
        s_jsonServiceMock.Object,
        s_kafkaServiceMock.Object);

    private static V77ApplicationProducerSettings TestSettings => new()
    {
        InfobasePath = "TestInfobasePath",
        Username = "TestUser",
        Password = "TestPassword",
        ObjectFilters = ["Id1:3", "Id2:2"],
        TransactionTypeFilters = ["Type1", "Type2"],
        DataTypePropertyName = "TestDatatype",
        ErtRelativePath = "Erts/test.ert",
    };

    private static List<LogTransaction> TestLogTransactions => [new LogTransaction()
    {
        ObjectId = "FakeObjectId",
        ObjectName = "FakeObjectName",
        Type = "FakeTransactionType",
    }];

    [TestMethod]
    public async Task GetLogTransactionsTask_ShouldGetLogTransactions()
    {
        V77ApplicationProducerSettings settings = TestSettings;

        // Setting up offset service mock
        Mock<IOffsetService> offsetServiceMock = new();
        offsetServiceMock
            .Setup(s => s.GetOffset(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("1234&FakeLastReadLine");

        // Setting up log service mock
        Mock<IV77ApplicationLogService> logServiceMock = new();
        long endPosition = 4321;
        string endLine = "NextFakeLastReadLine";
        GetLogTransactionsResult result = new()
        {
            LastReadOffset = new()
            {
                Position = endPosition,
                LastReadLine = endLine
            },
            Transactions = TestLogTransactions,
        };
        logServiceMock
            .Setup(s => s.GetLogTransactions(It.IsAny<string>(), It.IsAny<TransactionFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Setting up logger mock
        Mock<ILogger> loggerMock = new();

        GetLogTransactionsResult logTransactions = await s_service.GetLogTransactionsTask(
            settings,
            offsetServiceMock.Object,
            logServiceMock.Object,
            loggerMock.Object,
            cancellationToken: default);

        Assert.AreEqual(1, logTransactions.Transactions.Count);
    }

    [TestMethod]
    public async Task GetObjectJsonsTask_ShouldGetObjectJsons()
    {
        V77ApplicationProducerSettings settings = TestSettings;
        List<LogTransaction> logTransactions = TestLogTransactions;

        ObjectFilter objectFilter = new()
        {
            Id = "Id1",
            Depth = 3,
        };
        List<ObjectFilter> objectFilters = [objectFilter];

        // Setting up connection mock
        Mock<IComV77ApplicationConnection> connectionMock = new();
        connectionMock
            .Setup(c => c.RunErtAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("{}");

        // Setting up connection factory mock
        Mock<IComV77ApplicationConnectionFactory> connectionFactoryMock = new();
        connectionFactoryMock
            .Setup(f => f.GetConnectionAsync(It.IsAny<ConnectionProperties>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(connectionMock.Object);

        // Setting up logger mock
        Mock<ILogger> loggerMock = new();

        List<string> objectJsons = await s_service.GetObjectJsonsTask(
            settings,
            logTransactions,
            objectFilters,
            connectionFactoryMock.Object,
            loggerMock.Object,
            cancellationToken: default);

        Assert.AreEqual(logTransactions.Count, objectJsons.Count);
    }

    [TestMethod]
    public async Task SendObjectJsonsTask_ShouldPrepareAndSendObjectJsons()
    {
        string dataTypeJsonPropertyName = "TestDatatype";
        V77ApplicationProducerSettings settings = new()
        {
            InfobasePath = @"D:\Bases\TestInfobase",
            Username = null!,
            Password = null!,
            ObjectFilters = null!,
            TransactionTypeFilters = null!,
            DataTypePropertyName = dataTypeJsonPropertyName,
            ErtRelativePath = null!,
        };
        string objectDate = "01.01.2024";
        string transactionType = "TestTransactionType";
        List<LogTransaction> logTransactions = [new LogTransaction()
        {
            ObjectId = null!,
            ObjectName = "FakeObjectName " + objectDate,
            Type = transactionType,
        }];
        Dictionary<string, object?> expectedPropertiesToAdd = new()
        {
            { TransactionTypePropertyName, transactionType },
            { ObjectDatePropertyName, objectDate },
        };
        string objectJson = "{}";
        List<string> objectJsons = [objectJson];

        // Setting up json service mock
        Mock<IJsonService> jsonServiceMock = new();
        jsonServiceMock
            .Setup(s => s.BuildV77ApplicationProducerMessageData(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>(), It.IsAny<string>()))
            .Returns(new V77ApplicationProducerMessageData() { ObjectJson = "{\"TestObject\":\"TestValue\"}", DataType = null! });

        // Setting up kafka producer
        Mock<IProducer<string, string>> kafkaProducerMock = new();

        // Setting up kafka service mock
        Mock<IKafkaService> kafkaServiceMock = new();
        kafkaServiceMock
            .Setup(s => s.GetProducer<string, string>())
            .Returns(kafkaProducerMock.Object);
        string topicName = "TestTopicName";
        kafkaServiceMock
            .Setup(s => s.BuildTopicName(It.IsAny<string[]>()))
            .Returns(topicName);

        await s_service.SendObjectJsonsTask(
            settings,
            logTransactions,
            objectJsons,
            jsonServiceMock.Object,
            kafkaServiceMock.Object,
            cancellationToken: default);

        jsonServiceMock.Verify(s => s.BuildV77ApplicationProducerMessageData(
            It.Is<string>(s => s == objectJson),
            It.Is<Dictionary<string, object?>>(d => d.ToString() == expectedPropertiesToAdd.ToString()),
            It.Is<string>(s => s == dataTypeJsonPropertyName)),
            Times.Once);
        kafkaServiceMock.Verify(s => s.BuildTopicName(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        kafkaProducerMock.Verify(p => p.ProduceAsync(
            It.Is<string>(t => t == topicName),
            It.IsAny<Message<string, string>>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
