using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace KrasnyyOktyabr.ApplicationNet48.Services;

public sealed class OffsetService : IOffsetService
{
    public static string OffsetsFilePath => "offsets.json";

    private static readonly JsonSerializerOptions s_jsonSerializedOptions = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // To print cyrillic letters
    };

    /// <summary>
    /// Prevents race conditions.
    /// </summary>
    private readonly SemaphoreSlim _accessLock = new(1, 1);

#nullable enable
    /// <returns><c>null</c> when offset with corresponding <paramref name="key"/> not found.</returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="FailedToDeserializeOffsetsFileException"></exception>
    public async Task<string?> GetOffset(string key, CancellationToken cancellationToken = default)
    {
        await _accessLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            using FileStream stream = File.OpenRead(OffsetsFilePath);

            Dictionary<string, string>? offsets = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream, cancellationToken: cancellationToken)
                ?? throw new FailedToDeserializeOffsetsFileException();

            return offsets.TryGetValue(key, out string? offset) ? offset : null;
        }
        finally
        {
            _accessLock.Release();
        }
    }
#nullable disable

    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="FailedToDeserializeOffsetsFileException"></exception>
    /// <exception cref="FailedToSaveOffsetException"></exception>
    public async Task CommitOffset(string key, string offset, CancellationToken cancellationToken = default)
    {
        await _accessLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            using FileStream stream = File.Open(OffsetsFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            Dictionary<string, string> offsets = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream, cancellationToken: cancellationToken)
                    ?? throw new FailedToDeserializeOffsetsFileException();

            offsets[key] = offset;

            stream.SetLength(0); // Truncate file

            await JsonSerializer.SerializeAsync(stream, offsets, s_jsonSerializedOptions, cancellationToken);
        }
        finally
        {
            _accessLock.Release();
        }
    }

    public class FailedToDeserializeOffsetsFileException : Exception
    {
        internal FailedToDeserializeOffsetsFileException()
            : base($"Failed to deserialize '{OffsetsFilePath}'")
        {
        }
    }

    public class FailedToSaveOffsetException : Exception
    {
        internal FailedToSaveOffsetException(string key, string offset, Exception innerException)
            : base($"Failed to save offset '{offset}' with key '{key}'", innerException)
        {
        }
    }
}
