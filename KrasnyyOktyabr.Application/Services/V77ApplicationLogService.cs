using System.Text;
using KrasnyyOktyabr.Application.Logging;
using static KrasnyyOktyabr.Application.Services.IV77ApplicationLogService;

namespace KrasnyyOktyabr.Application.Services;

public sealed class V77ApplicationLogService(ILogger<V77ApplicationLogService> logger) : IV77ApplicationLogService
{
    public static long SeekBackBytesLimit => 10_000 * 1024;

    public static long MinSeekBackBytes => 5 * 1024;

    private static long LogFileBinarySearchDelta => 1024;

    private static char LogTransactionValueSeparator => ';';

    public async Task<GetLogTransactionsResult> GetLogTransactionsAsync(string logFilePath, TransactionFilterWithCommit filter, CancellationToken cancellationToken)
    {
        List<LogTransaction> logTransactions = [];

        await using FileStream fileStream = File.Open(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using StreamReader reader = new(fileStream, Encoding.GetEncoding(1251));

        fileStream.Position = CalculateStartPosition(fileStream.Length, filter.SeekBackPosition);

        logger.StartReading(fileStream.Position);

        int lineReadCount = 0;
        long endPosition = fileStream.Position;
        string endLine = string.Empty;

        while (!reader.EndOfStream)
        {
            endLine = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) ?? string.Empty;
            endPosition = fileStream.Position;
            lineReadCount++;

            if (lineReadCount == 2)
            {
                logger.FirstLineRead(endLine);
            }

            if (TryParseLogTransaction(endLine, filter, out LogTransaction? logTransaction))
            {
                logTransactions.Add(logTransaction!.Value);
            }

            if (endLine == filter.CommittedLine)
            {
                logger.CommittedLineFound(filter.CommittedLine, logTransactions.Count);
                logTransactions.Clear();
            }
        }

        logger.LastReadLine(lineReadCount, endLine, logTransactions.Count);

        return new()
        {
            Transactions = logTransactions,
            LastReadOffset = new()
            {
                Position = endPosition,
                LastReadLine = endLine,
            },
        };
    }

    /// <remarks>
    /// If <paramref name="filterStartPosition"/> is specified, it is substracted by <see cref="MinSeekBackBytes"/>.
    /// </remarks>
    /// <returns>The most recent position in file not exceeding <see cref="SeekBackBytesLimit"/>.</returns>
    public long CalculateStartPosition(long fileStreamLength, long? filterStartPosition)
    {
        if (filterStartPosition == null)
        {
            logger.NoFilterStartPosition();

            return fileStreamLength > SeekBackBytesLimit
                ? fileStreamLength - SeekBackBytesLimit
                : 0;
        }
        else
        {
            long estimatedPosition = filterStartPosition.Value - MinSeekBackBytes;

            if (estimatedPosition < 0)
            {
                estimatedPosition = 0;
            }

            if (fileStreamLength > SeekBackBytesLimit)
            {
                long minAvailablePosition = fileStreamLength - SeekBackBytesLimit;

                return estimatedPosition > minAvailablePosition
                    ? estimatedPosition
                    : minAvailablePosition;
            }
            else
            {
                logger.FileLengthIsLessThanSeekBackBytesLimit(fileStreamLength, SeekBackBytesLimit);

                return estimatedPosition > 0
                    ? estimatedPosition
                    : 0;
            }
        }
    }

    public async Task<long> SearchPositionByPrefixAsync(FileStream fileStream, string prefix, CancellationToken cancellationToken = default)
    {
        using StreamReader reader = new(fileStream, Encoding.GetEncoding(1251), bufferSize: 128, leaveOpen: true);

        long left = 0;
        long right = fileStream.Length;
        string lastReadLine = string.Empty;

        while (right - left > LogFileBinarySearchDelta)
        {
            reader.DiscardBufferedData(); // To prevent reading from buffer

            long middle = (left + right) / 2;

            reader.BaseStream.Position = middle;

            await reader.ReadLineAsync(cancellationToken); // Skip incomplete line

            lastReadLine = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) ?? string.Empty;

            if (prefix.CompareTo(lastReadLine) > 0)
            {
                left = middle;
            }
            else
            {
                right = middle;
            }
        }

        reader.BaseStream.Position = left;
        reader.DiscardBufferedData();
        await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);

        // Now lastReadLine <= prefix, left < searched position
        long previouslyReadPosition = left;

        do
        {
            previouslyReadPosition = reader.BaseStream.Position;

            lastReadLine = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) ?? string.Empty;
        }
        while (!lastReadLine.StartsWith(prefix) && prefix.CompareTo(lastReadLine) > 0 && !reader.EndOfStream);

        return previouslyReadPosition;
    }

    /// <exception cref="IOException"></exception>
    public async Task<GetLogTransactionsResult> GetLogTransactionsForPeriodAsync(string logFilePath, TransactionFilter filter, DateTimeOffset start, TimeSpan duration, CancellationToken cancellationToken = default)
    {
        List<LogTransaction> logTransactions = [];

        using FileStream fileStream = File.OpenRead(logFilePath);

        string startPrefix = FormatDateTime(start);

        long startPosition = await SearchPositionByPrefixAsync(fileStream, startPrefix, cancellationToken);

        fileStream.Position = startPosition;
        using StreamReader reader = new(fileStream, Encoding.GetEncoding(1251), bufferSize: 128);

        string endPrefix = FormatDateTime(start + duration);

        long lastReadPosition = fileStream.Position;
        string lastReadLine = string.Empty;

        while (!reader.EndOfStream)
        {
            lastReadLine = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) ?? string.Empty;
            lastReadPosition = fileStream.Position;

            if (endPrefix.CompareTo(lastReadLine) <= 0)
            {
                break;
            }

            if (TryParseLogTransaction(lastReadLine, filter, out LogTransaction? logTransaction))
            {
                logTransactions.Add(logTransaction!.Value);
            }
        }

        return new()
        {
            Transactions = logTransactions,
            LastReadOffset = new()
            {
                Position = lastReadPosition,
                LastReadLine = lastReadLine,
            },
        };
    }

    private static bool TryParseLogTransaction(string line, TransactionFilter filter, out LogTransaction? logTransaction)
    {
        logTransaction = null;

        string[] subs = line.Split(LogTransactionValueSeparator);

        if (subs.Length < 10)
        {
            return false;
        }

        string transactionType = subs[5];
        string objectId = subs[8];
        string objectName = subs[9];

        foreach (string objectIdToFilter in filter.ObjectIds)
        {
            foreach (string transactionTypeFilter in filter.TransactionTypes)
            {
                if (objectId.StartsWith(objectIdToFilter) && transactionType.StartsWith(transactionTypeFilter))
                {
                    logTransaction = new()
                    {
                        ObjectId = objectId,
                        ObjectName = objectName,
                        Type = transactionType,
                    };

                    return true;
                }
            }
        }

        return false;
    }

    /// <returns>Date in format like in 1C7 log file.</returns>
    private static string FormatDateTime(DateTimeOffset dateTime)
    {
        return dateTime.ToString("yyyyMMdd;HH:mm:ss;");
    }
}
