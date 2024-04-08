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
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty.</exception>
    /// <exception cref="ArgumentNullException"></exception>
    JToken? InputSelect(string path);

    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
    /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is negative.</exception>
    void OutputAdd(string key, object? value, int index);

    /// <returns>Deep copy of transformations result.</returns>
    JObject[] OutputGet();

    /// <exception cref="ArgumentNullException"></exception>
    void UpdateForeachCursor(string name, object? cursor, int index);

    /// <exception cref="ArgumentNullException"></exception>
    void ClearForeachCursor(string name);

    public class MemoryValueNotFoundException(string name) : Exception(name)
    {
    }
}
