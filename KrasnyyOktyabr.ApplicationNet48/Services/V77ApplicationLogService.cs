using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using static KrasnyyOktyabr.ApplicationNet48.Services.IV77ApplicationLogService;

namespace KrasnyyOktyabr.ApplicationNet48.Services;

public sealed class V77ApplicationLogService(ILogger<V77ApplicationLogService> logger) : IV77ApplicationLogService
{
    public static long SeekBackBytesLimit => 10_000 * 1024;

    public static long MinSeekBackBytes => 5 * 1024;

    private static long LogFileBinarySearchDelta => 1024;

    private static char LogTransactionValueSeparator => ';';

    public static string LogFileRelativePath => @"SYSLOG\1cv7.mlg";

    public static Regex ObjectDateRegex = new(@"\s+(\d{2}\.\d{2}\.\d{4})\s?");

    public async Task<GetLogTransactionsResult> GetLogTransactionsAsync(string logFilePath, TransactionFilterWithCommit filter, CancellationToken cancellationToken)
    {
        List<LogTransaction> logTransactions = [];

        using FileStream fileStream = File.Open(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using StreamReader reader = new(fileStream, Encoding.GetEncoding(1251));

        fileStream.Position = CalculateStartPosition(fileStream.Length, filter.SeekBackPosition);

        logger.LogTrace("Start reading from position {Position}", fileStream.Position);

        int lineReadCount = 0;
        long endPosition = fileStream.Position;
        string endLine = string.Empty;

        while (!reader.EndOfStream)
        {
            endLine = await reader.ReadLineAsync().ConfigureAwait(false) ?? string.Empty;
            endPosition = fileStream.Position;
            lineReadCount++;

            if (lineReadCount == 2)
            {
                logger.LogTrace("First line read: '{EndLine}'", endLine);
            }

            if (TryParseLogTransaction(endLine, filter, out LogTransaction? logTransaction))
            {
                logTransactions.Add(logTransaction!.Value);
            }

            if (endLine == filter.CommittedLine)
            {
                logger.LogTrace("Committed line found '{committedLine}' ({transactionsCleared} found transactions cleared)", filter.CommittedLine, logTransactions.Count);
                logTransactions.Clear();
            }
        }

        logger.LogTrace("{LinesReadCount} lines read, last line: '{EndLine}' ({TransactionsCount} transactions found)", lineReadCount, endLine, logTransactions.Count);

        return new(
            transactions: logTransactions,
            lastReadOffset: new(
                position: endPosition,
                lastReadLine: endLine
            )
        );
    }

    /// <remarks>
    /// If <paramref name="filterStartPosition"/> is specified, it is substracted by <see cref="MinSeekBackBytes"/>.
    /// </remarks>
    /// <returns>The most recent position in file not exceeding <see cref="SeekBackBytesLimit"/>.</returns>
    public long CalculateStartPosition(long fileStreamLength, long? filterStartPosition)
    {
        if (filterStartPosition == null)
        {
            logger.LogWarning("No filter start position");

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
                logger.LogTrace("File length ({FileStreamLength}) is less than limit ({SeekBackBytesLimits})", fileStreamLength, SeekBackBytesLimit);

                return estimatedPosition > 0
                    ? estimatedPosition
                    : 0;
            }
        }
    }

    public async Task<long> SearchPositionByPrefixAsync(FileStream fileStream, string prefix, CancellationToken cancellationToken = default)
    {
        using StreamReader reader = new(fileStream, Encoding.GetEncoding(1251), bufferSize: 128, leaveOpen: true, detectEncodingFromByteOrderMarks: false);

        long left = 0;
        long right = fileStream.Length;
        string lastReadLine = string.Empty;

        int iterationsCount = 0;

        while (right - left > LogFileBinarySearchDelta)
        {
            iterationsCount++;

            reader.DiscardBufferedData(); // To prevent reading from buffer

            long middle = (left + right) / 2;

            reader.BaseStream.Position = middle;

            await reader.ReadLineAsync(); // Skip incomplete line

            lastReadLine = await reader.ReadLineAsync().ConfigureAwait(false) ?? string.Empty;

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
        await reader.ReadLineAsync().ConfigureAwait(false);

        // Now lastReadLine <= prefix, left < searched position
        long previouslyReadPosition = left;

        do
        {
            iterationsCount++;

            previouslyReadPosition = reader.BaseStream.Position;

            lastReadLine = await reader.ReadLineAsync().ConfigureAwait(false) ?? string.Empty;
        }
        while (!lastReadLine.StartsWith(prefix) && prefix.CompareTo(lastReadLine) > 0 && !reader.EndOfStream);

        logger.LogTrace("Found the last line before '{Prefix}' for {IterationsCount} at {Position}: '{Line}'", prefix, iterationsCount, previouslyReadPosition, lastReadLine);

        return previouslyReadPosition;
    }

    /// <exception cref="IOException"></exception>
    public async Task<GetLogTransactionsResult> GetLogTransactionsForPeriodAsync(string logFilePath, TransactionFilter filter, DateTimeOffset start, TimeSpan duration, CancellationToken cancellationToken = default)
    {
        List<LogTransaction> logTransactions = [];

        using FileStream fileStream = File.Open(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        string startPrefix = FormatDateTime(start);

        long startPosition = await SearchPositionByPrefixAsync(fileStream, startPrefix, cancellationToken);

        fileStream.Position = startPosition;
        using StreamReader reader = new(fileStream, Encoding.GetEncoding(1251));

        DateTimeOffset end = start + duration;

        logger.LogTrace("Start reading period ({Start} - {End}) from position {Position}", start, end, fileStream.Position);

        string endPrefix = FormatDateTime(end);

        int lineReadCount = 0;
        long lastReadPosition = fileStream.Position;
        string lastReadLine = string.Empty;

        while (!reader.EndOfStream)
        {
            lineReadCount++;

            lastReadLine = await reader.ReadLineAsync().ConfigureAwait(false) ?? string.Empty;
            lastReadPosition = fileStream.Position;

            if (lineReadCount == 1)
            {
                logger.LogTrace("First line read in period ({Start} - {End}): '{Line}'", start, end, lastReadLine);
            }

            if (endPrefix.CompareTo(lastReadLine) <= 0)
            {
                break;
            }

            if (TryParseLogTransaction(lastReadLine, filter, out LogTransaction? logTransaction))
            {
                logTransactions.Add(logTransaction!.Value);
            }
        }

        logger.LogTrace("{LinesReadCound} lines read in period ({Start} - {End}), last line: '{LastReadLine}' ({TransactionsCount} transactions found)", lineReadCount, start, end, lastReadLine, logTransactions.Count);

        return new(
            transactions: logTransactions,
            lastReadOffset: new(
                position: lastReadPosition,
                lastReadLine: lastReadLine
            )
        );
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
                    logTransaction = new(
                        objectId: objectId,
                        objectName: objectName,
                        type: transactionType
                    );

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
