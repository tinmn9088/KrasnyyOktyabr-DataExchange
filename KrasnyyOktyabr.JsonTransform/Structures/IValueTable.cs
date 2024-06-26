namespace KrasnyyOktyabr.JsonTransform.Structures;

public interface IValueTable : IEnumerable<List<object?>>
{
    int Count { get; }

    /// <summary>
    /// Add and select new line.
    /// </summary>
    void AddLine();

    /// <summary>
    /// Add column and fill it with empty values.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    void AddColumn(string column);

    /// <summary>
    /// Set pointer to the line with <paramref name="index"/> (starts with <c>0</c>).
    /// </summary>
    void SelectLine(int index);

    /// <exception cref="LineNotSelectedException"></exception>
    /// <exception cref="CoulmnNotFoundException"></exception>
    void SetValue(string column, object? value);

    /// <exception cref="LineNotSelectedException"></exception>
    /// <exception cref="CoulmnNotFoundException"></exception>
    object? GetValue(string column);

    /// <summary>
    /// Collapse the table by the corresponding <paramref name="columnsToGroup"/>,
    /// i.e. replaces all duplicate rows (by grouping <paramref name="columnsToGroup"/>) with one row,
    /// summing <paramref name="columnsToSum"/> values.
    /// </summary>
    void Collapse(IEnumerable<string> columnsToGroup, IEnumerable<string> columnsToSum);

    public class LineNotSelectedException : Exception
    {
    }

    public class CoulmnNotFoundException(string name) : Exception($"Column '{name}' not found")
    {
    }
}
