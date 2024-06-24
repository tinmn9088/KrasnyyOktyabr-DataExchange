using System.Collections;
using static KrasnyyOktyabr.JsonTransform.Structures.IValueTable;

namespace KrasnyyOktyabr.JsonTransform.Structures;

public sealed class ValueTable : IValueTable
{
    private const int NOT_SELECTED_LINE_INDEX = -1;

    private List<List<object?>> _values;

    private List<string> _columns;

    private int _currentLineIndex;

    public int Count => _values.Count;

    public ValueTable(IEnumerable<string> columns)
    {
        _columns = columns.ToList();

        if (_columns.Count == 0)
        {
            throw new ArgumentException("No columns provided");
        }

        _values = [];
        _currentLineIndex = NOT_SELECTED_LINE_INDEX;
    }

    public void AddLine()
    {
        List<object?> emptyLine = Enumerable.Repeat<object?>(null, _columns.Count).ToList();

        _values.Add(emptyLine);

        _currentLineIndex = _values.Count - 1;
    }

    public void SelectLine(int index)
    {
        if (index < 0 || index >= _values.Count)
        {
            throw new IndexOutOfRangeException();
        }

        _currentLineIndex = index;
    }

    public void SetValue(string name, object? value)
    {
        CheckLineIsSelected();

        int columnIndex = GetColumnIndexByName(name);

        _values[_currentLineIndex][columnIndex] = value;
    }

    public object? GetValue(string name)
    {
        CheckLineIsSelected();

        int columnIndex = GetColumnIndexByName(name);

        return _values[_currentLineIndex][columnIndex];
    }

    public void Collapse(string[] columnsToGroup, string[] columnsToSum)
    {
        List<string> newColumns = [.. columnsToGroup, .. columnsToSum];

        List<List<object?>> collapsedValues = [];

        foreach (List<object?> line in _values)
        {
            // TODO: implement
            throw new NotImplementedException();
        }

        _columns = newColumns;
        _values = collapsedValues;
    }

    /// <exception cref="LineNotSelectedException"></exception>
    private void CheckLineIsSelected()
    {
        if (_currentLineIndex == NOT_SELECTED_LINE_INDEX)
        {
            throw new LineNotSelectedException();
        }
    }

    /// <exception cref="LineNotSelectedException"></exception>
    /// <exception cref="CoulmnNotFoundException"></exception>
    private int GetColumnIndexByName(string name)
    {
        int columnIndex = _columns.IndexOf(name);

        if (columnIndex == -1)
        {
            throw new CoulmnNotFoundException(name);
        }

        return columnIndex;
    }

    public IEnumerator<List<object?>> GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
