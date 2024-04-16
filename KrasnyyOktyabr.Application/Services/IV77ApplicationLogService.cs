namespace KrasnyyOktyabr.Application.Services;

public interface IV77ApplicationLogService
{
    public readonly struct LogTransaction()
    {
        public required string Type { get; init; }

        public required string ObjectId { get; init; }

        public required string ObjectName { get; init; }
    }

    public readonly struct TransactionFilter
    {
        public required long? SeekBackPosition { get; init; }

        public required string CommittedLine { get; init; }

        public required string[] ObjectIds { get; init; }

        public required string[] TransactionTypes { get; init; }
    }

    public readonly struct LogOffset
    {
        public required long? Position { get; init; }

        public required string LastReadLine { get; init; }
    }

    public readonly struct GetLogTransactionsResult
    {
        public required LogOffset LastReadOffset { get; init; }

        public required List<LogTransaction> Transactions { get; init; }
    }

    Task<GetLogTransactionsResult> GetLogTransactions(string logFilePath, TransactionFilter filter, CancellationToken cancellationToken = default);
}
