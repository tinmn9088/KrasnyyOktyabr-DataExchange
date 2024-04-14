namespace KrasnyyOktyabr.Application.Services;

public interface IJsonService
{
    /// <exception cref="ArgumentException"></exception>
    string RemoveEmptyPropertiesAndAdd(string jsonObject, Dictionary<string, object?> propertiesToAdd);
}
