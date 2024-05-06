#nullable enable

using System;
using System.Collections.Generic;
using KrasnyyOktyabr.JsonTransform.Expressions.Creation;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static KrasnyyOktyabr.ApplicationNet48.Services.IJsonService;

namespace KrasnyyOktyabr.ApplicationNet48.Services.Tests;

[TestClass]
public class JsonServiceTests
{
    private static Mock<IJsonAbstractExpressionFactory>? s_factoryMock;

    private static Mock<ILogger<JsonService>>? s_loggerMock;

    private static JsonService? s_jsonService;

    [TestInitialize]
    public void TestInitialized()
    {
        s_factoryMock = new();

        s_loggerMock = new();

        s_jsonService = new(s_factoryMock.Object, s_loggerMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void BuildV77ApplicationProducerMessageData_WhenObjectJsonNull_ShouldThrowArgumentNullException()
    {
        s_jsonService!.BuildKafkaProducerMessageData(null!, [], string.Empty);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void BuildV77ApplicationProducerMessageData_WhenPropertiesToAddNull_ShouldThrowArgumentNullException()
    {
        s_jsonService!.BuildKafkaProducerMessageData(string.Empty, null!, string.Empty);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void BuildV77ApplicationProducerMessageData_WhenDataTypePropertyNameNull_ShouldThrowArgumentNullException()
    {
        s_jsonService!.BuildKafkaProducerMessageData(string.Empty, [], null!);
    }

    [TestMethod]
    [ExpectedException(typeof(FailedToGetDataTypeException))]
    public void BuildV77ApplicationProducerMessageData_WhenPropertyWithDataTypePropertyNameNotPresent_ShouldFailedToGetDataTypeException()
    {
        s_jsonService!.BuildKafkaProducerMessageData("{}", [], "DataType");
    }

    [TestMethod]
    public void BuildV77ApplicationProducerMessageData_ShouldPrepareObjectJson()
    {
        string dataTypePropertyName = "DataType";
        string dataType = "TestType";
        string objectJson = "{\"" + dataTypePropertyName + "\":\"" + dataType + "\",\"NullProperty\":null}";

        Dictionary<string, object?> propertiesToAdd = new()
        {
            { "Property1", "TestValue1" },
            { "NewNullProperty", null },
        };

        KafkaProducerMessageData messageData = s_jsonService!.BuildKafkaProducerMessageData(objectJson, propertiesToAdd, dataTypePropertyName);

        Assert.AreEqual("{\"" + dataTypePropertyName + "\":\"" + dataType + "\",\"Property1\":\"TestValue1\",\"NewNullProperty\":null}", messageData.ObjectJson);
        Assert.AreEqual(dataType, messageData.DataType);
    }
}
