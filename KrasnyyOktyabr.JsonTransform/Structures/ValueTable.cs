using System.Collections;
using KrasnyyOktyabr.JsonTransform.Numerics;
using static KrasnyyOktyabr.JsonTransform.Structures.IValueTable;

namespace KrasnyyOktyabr.JsonTransform.Structures;

public sealed class ValueTable : IValueTable
{
    private const int NOT_SELECTED_LINE_INDEX = -1;

    private List<List<object?>> _values;

    private List<string> _columns;

    private int _currentLineIndex;

    public int Count => _values.Count;

    public IReadOnlyList<string> Columns => _columns.AsReadOnly();

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

        emptyLine.TrimExcess();

        _values.Add(emptyLine);

        _currentLineIndex = _values.Count - 1;
    }

    public void AddColumn(string column)
    {
        if (column is null)
        {
            throw new ArgumentNullException(nameof(column));
        }

        _columns.Add(column);

        foreach (List<object?> line in _values)
        {
            line.Add(null);
        }
    }

    public void SelectLine(int index)
    {
        if (index < 0 || index >= _values.Count)
        {
            throw new IndexOutOfRangeException();
        }

        _currentLineIndex = index;
    }

    public void SetValue(string column, object? value)
    {
        CheckLineIsSelected();

        int columnIndex = GetColumnIndexByName(column);

        _values[_currentLineIndex][columnIndex] = value;
    }

    public object? GetValue(string column)
    {
        CheckLineIsSelected();

        int columnIndex = GetColumnIndexByName(column);

        return _values[_currentLineIndex][columnIndex];
    }

    public void Collapse(IEnumerable<string> columnsToGroup, IEnumerable<string>? columnsToSum)
    {
        Dictionary<CollapseKey, List<Number>> valuesToGroupToSum = [];

        for (int i = 0; i < Count; i++)
        {
            CollapseKey key = CollapseKey.ExtractFromValueTable(this, columnsToGroup, i);

            List<Number> valuesToSum = columnsToSum
                .Select(GetValue)
                .Select(value => Number.TryParse(value?.ToString(), out Number number)
                    ? number
                    : throw new InvalidCastException($"Cannot cast to number: {value}"))
                .ToList();

            valuesToGroupToSum[key] = valuesToGroupToSum.TryGetValue(key, out List<Number> sum)
                ? sum.Zip(valuesToSum, (left, right) => left + right)
                    .ToList()
                : valuesToSum;
        }

        _currentLineIndex = NOT_SELECTED_LINE_INDEX;

        _columns = [.. columnsToGroup, .. columnsToSum];

        static List<object?> JoinKeyAndCollapsedValues(CollapseKey key, List<Number> sum) => [.. key.Value, .. sum];

        _values = valuesToGroupToSum
            .Select(keySum => JoinKeyAndCollapsedValues(keySum.Key, keySum.Value))
            .ToList();
    }

    public void Collapse(IEnumerable<string> columnsToGroup)
    {
        HashSet<CollapseKey> valuesToGroup = [];

        for (int i = 0; i < Count; i++)
        {
            CollapseKey key = CollapseKey.ExtractFromValueTable(this, columnsToGroup, i);

            valuesToGroup.Add(key);
        }

        _columns = [.. columnsToGroup];

        static List<object?> ExtractKeyValues(CollapseKey key) => [.. key.Value];

        _values = valuesToGroup
            .Select(ExtractKeyValues)
            .ToList();
    }

    public IEnumerator<List<object?>> GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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

    private class CollapseKey : IEquatable<CollapseKey>
    {
        private readonly IReadOnlyList<object?> _value;

        private CollapseKey(IReadOnlyList<object?> value)
        {
            _value = value;
        }

        public IReadOnlyList<object?> Value => _value;

        public static CollapseKey ExtractFromValueTable(ValueTable valueTable, IEnumerable<string> columnsToGroup, int lineNumber)
        {
            valueTable.SelectLine(lineNumber);

            return new CollapseKey(columnsToGroup
                .Select(valueTable.GetValue)
                .ToList()
                .AsReadOnly());
        }

        public bool Equals(CollapseKey other) => _value.SequenceEqual(other._value);

        public override bool Equals(object obj) => obj is CollapseKey other && Equals(other);

        public override int GetHashCode() => _value.Sum(v => v?.GetHashCode() ?? 0);
    }
}
