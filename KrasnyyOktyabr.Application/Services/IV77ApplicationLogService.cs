using System.Text.RegularExpressions;

namespace KrasnyyOktyabr.Application.Services;

public partial interface IV77ApplicationLogService
{
    public readonly struct LogTransaction()
    {
        public required string Type { get; init; }

        public required string ObjectId { get; init; }

        public required string ObjectName { get; init; }
    }

    public class TransactionFilterWithCommit : TransactionFilter
    {
        public required long? SeekBackPosition { get; init; }

        public required string CommittedLine { get; init; }
    }

    public class TransactionFilter
    {
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

    public static string LogFileRelativePath => @"SYSLOG\1cv7.mlg";

    [GeneratedRegex(@"\s+(\d{2}\.\d{2}\.\d{4})\s?")]
    public static partial Regex ObjectDateRegex();

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
