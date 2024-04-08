using KrasnyyOktyabr.JsonTransform.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.IContext;

namespace KrasnyyOktyabr.JsonTransform;

public sealed class Context : IContext
{
    /// <summary>
    /// Supports proper serialization of <see cref="Number"/>.
    /// </summary>
    private static readonly JsonSerializer s_jsonSerializer = JsonSerializer.Create(new() { Converters = [new NumberJsonConverter()] });

    /// <summary>
    /// JSON to be transformed.
    /// </summary>
    private readonly JObject _input;

    /// <summary>
    /// Transformation result.
    /// </summary>
    private readonly List<JObject> _output;

    /// <summary>
    /// Storage for variables. <c>null</c> or empty variable names are not allowed.
    /// </summary>
    private readonly Dictionary<string, object?> _memory;

    private readonly Dictionary<string, (object?, int)> _cursors;

    private readonly List<string> _cursorNames;

    /// <exception cref="ArgumentNullException"></exception>
    public Context(JObject input)
    {
        ArgumentNullException.ThrowIfNull(input);

        _input = input;
        _memory = [];
        _output = [];
        _cursors = [];
        _cursorNames = [];
    }

    public void MemorySet(string name, object? value)
    {
        ArgumentNullException.ThrowIfNull(name);

        _memory[name] = value;
    }

    public object? MemoryGet(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return _memory.TryGetValue(name, out object? value)
            ? value
            : throw new IContext.MemoryValueNotFoundException(name);
    }

    public JToken? InputSelect(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        try
        {
            return _input.SelectToken(path);
        }
        catch (JsonException)
        {
            return new JArray(_input.SelectTokens(path));
        }
    }

    public void OutputAdd(string key, object? value, int index)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (index < 0)
        {
            throw new IndexOutOfRangeException();
        }

        JObject outputItem;

        if (index >= _output.Count)
        {
            int missingItemsCount = index + 1 - _output.Count;
            List<JObject> missingObjects = new(missingItemsCount);

            for (int i = 0; i < missingItemsCount; i++)
            {
                missingObjects.Add(new JObject());
            }

            _output.AddRange(missingObjects);
        }

        outputItem = _output[index];

        outputItem.Add(key, value != null ? JToken.FromObject(value, s_jsonSerializer) : JValue.CreateNull());
    }

    public JObject[] OutputGet()
    {
        return _output.Select(outputItem => (JObject)outputItem.DeepClone()).ToArray();
    }

    public void UpdateCursor(string name, object? cursor, int index)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (!_cursors.ContainsKey(name))
        {
            _cursorNames.Add(name);
        }

        _cursors[name] = (cursor, index);
    }

    public object? GetCursor()
    {
        if (!TryGetLastAddedCursorName(out string? name))
        {
            return CursorNotFound;
        }

        bool isPresent = _cursors.TryGetValue(name!, out (object?, int) cursorIndex);

        return isPresent
            ? cursorIndex.Item1
            : CursorNotFound;
    }

    public object? GetCursor(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        bool isPresent = _cursors.TryGetValue(name, out (object?, int) cursorIndex);

        return isPresent
            ? cursorIndex.Item1
            : CursorNotFound;
    }

    public int GetCursorIndex()
    {
        if (!TryGetLastAddedCursorName(out string? name))
        {
            return CursorIndexNotFound;
        }

        bool isPresent = _cursors.TryGetValue(name!, out (object?, int) cursorIndex);

        return isPresent
            ? cursorIndex.Item2
            : CursorIndexNotFound;
    }

    public int GetCursorIndex(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        bool isPresent = _cursors.TryGetValue(name, out (object?, int) cursorIndex);

        return isPresent
            ? cursorIndex.Item2
            : CursorIndexNotFound;
    }

    public void RemoveCursor(string name)
    {
        _cursorNames.Remove(name);
        _cursors.Remove(name);
    }

    private bool TryGetLastAddedCursorName(out string? name)
    {
        name = null;

        if (!_cursorNames.Any())
        {
            return false;
        }

        name = _cursorNames[_cursorNames.Count - 1];

        return true;
    }
}
