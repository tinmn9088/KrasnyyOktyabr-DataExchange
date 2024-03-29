using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform;

public interface IContext
{
    void MemorySet(string name, object value);

    object MemoryGet(string name);

    JToken? InputSelect(string path);

    void OutputAdd(string key, object value, int outputIndex);

    JObject[] OutputGet();
}
