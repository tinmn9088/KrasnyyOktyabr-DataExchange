using KrasnyyOktyabr.JsonTransform.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

    /// <exception cref="ArgumentNullException"></exception>
    public Context(JObject input)
    {
        ArgumentNullException.ThrowIfNull(input);

        _input = input;
        _memory = [];
        _output = [];
    }

    public void MemorySet(string name, object? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        _memory[name] = value;
    }

    public object? MemoryGet(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

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
            List<JObject> missingObjects = new List<JObject>(missingItemsCount);

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
}
