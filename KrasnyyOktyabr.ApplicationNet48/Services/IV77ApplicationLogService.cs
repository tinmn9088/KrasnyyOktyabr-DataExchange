using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KrasnyyOktyabr.ApplicationNet48.Services;

public partial interface IV77ApplicationLogService
{
    public readonly struct LogTransaction(string type, string objectId, string objectName)
    {
        public string Type { get; } = type;

        public string ObjectId { get; } = objectId;

        public string ObjectName { get; } = objectName;
    }

    public class TransactionFilterWithCommit(
        string[] objectIds,
        string[] transactionTypes,
        long? seekBackPosition,
        string committedLine)
        : TransactionFilter(objectIds, transactionTypes)
    {
        public long? SeekBackPosition { get; } = seekBackPosition;

        public string CommittedLine { get; } = committedLine;
    }

    public class TransactionFilter(string[] objectIds, string[] transactionTypes)
    {
        public string[] ObjectIds { get; } = objectIds;

        public string[] TransactionTypes { get; } = transactionTypes;
    }

    public readonly struct LogOffset(long? position, string lastReadLine)
    {
        public long? Position { get; } = position;

        public string LastReadLine { get; } = lastReadLine;
    }

    public readonly struct GetLogTransactionsResult(LogOffset lastReadOffset, List<LogTransaction> transactions)
    {
        public LogOffset LastReadOffset { get; } = lastReadOffset;

        public List<LogTransaction> Transactions { get; } = transactions;
    }

    Task<GetLogTransactionsResult> GetLogTransactionsAsync(string logFilePath, TransactionFilterWithCommit filter, CancellationToken cancellationToken);

    /// <summary>
    /// Searches for the first transaction in <paramref name="logFileStream"/> which line starts with <paramref name="prefix"/>.
    /// </summary>
    /// <remarks>
    /// Assumes that all lines in file are sorted and uses binary search.
    /// </remarks>
    Task<long> SearchPositionByPrefixAsync(FileStream fileStream, string prefix, CancellationToken cancellationToken);

    /// <summary>
    /// Searches for the first transaction in <paramref name="logFilePath"/> which has greater or equal timestamp.
    /// </summary>
    Task<GetLogTransactionsResult> GetLogTransactionsForPeriodAsync(string logFilePath, TransactionFilter filter, DateTimeOffset start, TimeSpan duration, CancellationToken cancellationToken);
}
