using System.Collections.Generic;

namespace KrasnyyOktyabr.Scripting.Core.Models;

public readonly struct JsonTransformMsSqlResult(string table, Dictionary<string, dynamic> columnValues)
{
    public string Table { get; } = table;

    public Dictionary<string, dynamic> ColumnValues { get; } = columnValues;
}