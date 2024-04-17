﻿using System.Text;
using Microsoft.Extensions.Logging;
using Moq;
using static KrasnyyOktyabr.Application.Services.IV77ApplicationLogService;
using static KrasnyyOktyabr.Application.Services.V77ApplicationLogService;

namespace KrasnyyOktyabr.Application.Services.Tests;

[TestClass]
public class V77ApplicationLogServiceTests
{
    private static readonly Mock<ILogger<V77ApplicationLogService>> s_loggerMock = new();

    private static readonly V77ApplicationLogService s_logService = new(s_loggerMock.Object);

    private static string LogFilePath => Path.Combine("Resources", "V77ApplicationLogFile.mlg");

    [TestInitialize]
    public void TestInitialize()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    [TestMethod]
    public async Task ReadLogTransactions_ShouldReadLogTransactions()
    {
        TransactionFilter filter = new()
        {
            ObjectIds = ["O/666/", "O/999/"],
            TransactionTypes = ["DocWrite"],
            CommittedLine = "20230301;09:49:24;Пользователь_6;E;Docs;DocBackPassed;2;;O/666/444007;Документ3 96569 01.03.2023 09:49:22",
            SeekBackPosition = 100,
        };

        GetLogTransactionsResult result = await s_logService.GetLogTransactions(
            LogFilePath,
            filter,
            cancellationToken: default);

        Assert.AreEqual(4, result.Transactions.Count);
        Assert.AreEqual(new FileInfo(LogFilePath).Length, result.LastReadOffset.Position);
        Assert.AreEqual("20230301;09:49:31;Пользователь_8;E;Docs;DocWrite;2;;O/80/222234;Документ2 135262 16.11.2021 23:59:59", result.LastReadOffset.LastReadLine);
    }

    [TestMethod]
    public void CalculateStartPosition_WhenFilterStartPositionNull_ShouldCalculateStartPosition()
    {
        // length > limit => length - limit
        Assert.AreEqual(1, s_logService.CalculateStartPosition(SeekBackBytesLimit + 1, null));

        // length < limit => 0
        Assert.AreEqual(0, s_logService.CalculateStartPosition(SeekBackBytesLimit - 1, null));
    }

    [TestMethod]
    public void CalculateStartPosition_ShouldCalculateStartPosition()
    {
        // length > limit, filter < length - limit => length - limit - min
        Assert.AreEqual(1, s_logService.CalculateStartPosition(SeekBackBytesLimit + 1, 0));

        // length > limit, filter > length - limit => filter - min
        Assert.AreEqual(1, s_logService.CalculateStartPosition(SeekBackBytesLimit + 1, 2));

        // length < limit => filter - min
        Assert.AreEqual(0, s_logService.CalculateStartPosition(SeekBackBytesLimit - 1, 2));
    }
}