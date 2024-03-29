using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform;

/// <summary>
/// Used to store input, output and variables.
/// </summary>
public sealed class Context : IContext
{
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
    private readonly Dictionary<string, object> _memory;

    /// <exception cref="ArgumentNullException"></exception>
    public Context(JObject input)
    {
        ArgumentNullException.ThrowIfNull(input);

        _input = input;
        _memory = [];
        _output = [];
    }

    /// <exception cref="ArgumentException"><paramref name="name"/> is <c>null</c> or empty.</exception>
    public void MemorySet(string name, object value)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        _memory[name] = value;
    }

    /// <exception cref="ArgumentException"><paramref name="name"/> is empty.</exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="MemoryValueNotFoundException"></exception>
    public object MemoryGet(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        return _memory.TryGetValue(name, out object? value)
            ? value
            : throw new MemoryValueNotFoundException(name);
    }

    /// <returns>
    /// Result of JSONPath query or <c>null</c> if not found.
    /// </returns>
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

    /// <exception cref="ArgumentException"><paramref name="key"/> is empty.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
    /// <exception cref="IndexOutOfRangeException"><paramref name="outputIndex"/> is negative.</exception>
    public void OutputAdd(string key, object value, int outputIndex)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        if (outputIndex < 0)
        {
            throw new IndexOutOfRangeException();
        }

        JObject outputItem;

        if (outputIndex >= _output.Count)
        {
            int missingItemsCount = outputIndex + 1 - _output.Count;
            List<JObject> missingObjects = new List<JObject>(missingItemsCount);

            for (int i = 0; i < missingItemsCount; i++)
            {
                missingObjects.Add(new JObject());
            }

            _output.AddRange(missingObjects);
        }

        outputItem = _output[outputIndex];

        outputItem.Add(key, JToken.FromObject(value));
    }

    /// <returns>Deep copy of transformations result.</returns>
    public JObject[] OutputGet()
    {
        return _output.Select(outputItem => (JObject)outputItem.DeepClone()).ToArray();
    }

    public class MemoryValueNotFoundException : Exception
    {
        internal MemoryValueNotFoundException(string name) : base(name)
        {
        }
    }
}
