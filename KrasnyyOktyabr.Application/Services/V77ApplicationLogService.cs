using System.Text;
using KrasnyyOktyabr.Application.Logging;
using static KrasnyyOktyabr.Application.Services.IV77ApplicationLogService;

namespace KrasnyyOktyabr.Application.Services;

public sealed class V77ApplicationLogService(ILogger<V77ApplicationLogService> logger) : IV77ApplicationLogService
{
    public static long SeekBackBytesLimit => 10_000 * 1024;

    public static long MinSeekBackBytes => 5 * 1024;

    private static char LogTransactionValueSeparator => ';';

    public async Task<GetLogTransactionsResult> GetLogTransactions(string logFilePath, TransactionFilter filter, CancellationToken cancellationToken)
    {

        List<LogTransaction> logTransactions = [];

        using FileStream fileStream = File.Open(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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

            string[] subs = endLine.Split(LogTransactionValueSeparator);

            if (subs.Length < 10)
            {
                continue;
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
                        logTransactions.Add(new LogTransaction()
                        {
                            ObjectId = objectId,
                            ObjectName = objectName,
                            Type = transactionType,
                        });
                        goto LoopEnd;
                    }
                }
            }
        LoopEnd:;

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
}
