using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform;

/// <summary>
/// Used to store input, output and variables.
/// </summary>
public interface IContext
{
    /// <exception cref="ArgumentNullException"></exception>
    void MemorySet(string name, object? value);

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="MemoryValueNotFoundException"></exception>
    object? MemoryGet(string name);

    /// <returns>
    /// Result of JSONPath query or <c>null</c> if not found.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <c>null</c> or empty.</exception>
    JToken? InputSelect(string path);

    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
    /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is negative.</exception>
    void OutputAdd(string key, object? value, int index);

    List<JObject> OutputGet();

    /// <exception cref="ArgumentNullException"></exception>
    void UpdateCursor(string name, object? cursor, int index);

    /// <exception cref="CursorNotFoundException"></exception>
    object? GetCursor();

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="CursorNotFoundException"></exception>
    object? GetCursor(string name);

    /// <exception cref="CursorIndexNotFoundException"
    int GetCursorIndex();

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="CursorIndexNotFoundException"></exception>
    int GetCursorIndex(string name);

    /// <exception cref="ArgumentNullException"></exception>
    void RemoveCursor(string name);

    public class MemoryValueNotFoundException(string name) : Exception($"Value with name '{name}' not found")
    {
    }

    public class CursorNotFoundException : Exception
    {
        internal CursorNotFoundException()
        {
        }

        internal CursorNotFoundException(string name) : base($"Cursor not found for loop '{name}'")
        {
        }
    }

    public class CursorIndexNotFoundException : Exception
    {
        internal CursorIndexNotFoundException()
        {
        }

        internal CursorIndexNotFoundException(string name) : base($"Cursor index not found for loop '{name}'")
        {
        }
    }
}
