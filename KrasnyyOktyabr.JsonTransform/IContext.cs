using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform;

/// <summary>
/// Used to store input, output and variables.
/// </summary>
public interface IContext
{
    public static object? CursorNotFound => null;

    public static int CursorIndexNotFound => -1;

    /// <exception cref="ArgumentNullException"></exception>
    void MemorySet(string name, object? value);

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="MemoryValueNotFoundException"></exception>
    object? MemoryGet(string name);

    /// <returns>
    /// Result of JSONPath query or <c>null</c> if not found.
    /// </returns>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty.</exception>
    /// <exception cref="ArgumentNullException"></exception>
    JToken? InputSelect(string path);

    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
    /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is negative.</exception>
    void OutputAdd(string key, object? value, int index);

    /// <returns>Deep copy of transformations result.</returns>
    JObject[] OutputGet();

    /// <exception cref="ArgumentNullException"></exception>
    void UpdateCursor(string name, object? cursor, int index);

    /// <returns>Cursor value or <see cref="CursorNotFound"/>.</returns>
    object? GetCursor();

    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>Cursor value or <see cref="CursorNotFound"/>.</returns>
    object? GetCursor(string name);

    /// <returns>Cursor index or <see cref="CursorIndexNotFound"/>.</returns>
    int GetCursorIndex();

    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>Cursor index or <see cref="CursorIndexNotFound"/>.</returns>
    int GetCursorIndex(string name);

    /// <exception cref="ArgumentNullException"></exception>
    void RemoveCursor(string name);

    public class MemoryValueNotFoundException(string name) : Exception(name)
    {
    }
}
