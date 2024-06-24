namespace KrasnyyOktyabr.JsonTransform.Structures;

public interface IValueTable : IEnumerable<List<object?>>
{
    int Count { get; }

    /// <summary>
    /// Add and select new line.
    /// </summary>
    void AddLine();

    /// <summary>
    /// Set pointer to the line with <paramref name="index"/> (starts with <c>0</c>).
    /// </summary>
    void SelectLine(int index);

    /// <exception cref="LineNotSelectedException"></exception>
    /// <exception cref="CoulmnNotFoundException"></exception>
    void SetValue(string name, object? value);

    /// <exception cref="LineNotSelectedException"></exception>
    /// <exception cref="CoulmnNotFoundException"></exception>
    object? GetValue(string name);

    void Collapse(string[] columnsToGroup, string[] columnsToSum);

    public class LineNotSelectedException : Exception
    {
    }

    public class CoulmnNotFoundException(string name) : Exception($"Column '{name}' not found")
    {
    }
}
