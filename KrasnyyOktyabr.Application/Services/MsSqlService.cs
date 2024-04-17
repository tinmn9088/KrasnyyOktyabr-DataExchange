using System.Data.Common;
using System.Data.OleDb;
using System.Globalization;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using KrasnyyOktyabr.Application.Logging;

namespace KrasnyyOktyabr.Application.Services;

[SupportedOSPlatform("windows")]
public partial class MsSqlService(ILogger<MsSqlService> logger) : IMsSqlService
{
    private static readonly Regex[] s_illegalSequenceRegexes = [InsertCommandRegex(), UpdateCommandRegex(), DeleteCommandRegex(), DropCommandRegex()];

    /// <summary>
    /// C# types mappings in SQL query strings.
    /// </summary>
    private static readonly Dictionary<Predicate<dynamic>, Func<dynamic, string>> s_predicatesValueMappings = new()
    {
        { // NULL
            value => value is null,
            value => "NULL"
        },
        { // Strings
            value => value is string,
            value => $"'{value}'"
        },
        { // Numbers
            value => double.TryParse(value.ToString(), out double _),
            value => $"{value.ToString(CultureInfo.InvariantCulture)}"
        },
    };

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="MsSqlException"></exception>
    public async Task<object?> SelectSingleValueAsync(string connectionString, string query)
    {
        ValidateSelectQueryCommandText(query);

        try
        {
            await using OleDbConnection connection = new(connectionString);

            await connection.OpenAsync().ConfigureAwait(false);

            await using OleDbCommand command = new(query, connection);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            await using DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            return reader.HasRows && reader.Read()
                ? reader.GetValue(0)
                : null;
        }
        catch (Exception ex)
        {
            throw new MsSqlException(ex.Message, ex);
        }
    }

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="MsSqlException"></exception>
    public async Task InsertAsync(string connectionString, string table, Dictionary<string, dynamic> columnsValues)
    {
        string commandText = BuildInsertQueryText(table, columnsValues);

        try
        {
            await using OleDbConnection connection = new(connectionString);

            await connection.OpenAsync().ConfigureAwait(false);

            await using OleDbCommand command = new(commandText, connection);

            int insertedRowsCount = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            logger.Inserted(insertedRowsCount);
        }
        catch (Exception ex)
        {
            throw new MsSqlException(ex.Message, ex);
        }
    }

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ValueMappingForSqlCommandNotFoundException"></exception>
    public static string BuildInsertQueryText(string table, Dictionary<string, dynamic> columnsValues)
    {
        ValidateName(table);

        List<string> columns = new(columnsValues.Count);
        List<dynamic> values = new(columnsValues.Count);

        foreach (KeyValuePair<string, dynamic> columnValueEntry in columnsValues)
        {
            ValidateName(columnValueEntry.Key);
            columns.Add(columnValueEntry.Key);
            values.Add(columnValueEntry.Value);
        }

        string columnsString = string.Join(",", columns.Select(column => $"[{column}]"));
        string valuesString = string.Join(",", values.Select(value => MapValueForSqlCommand(value)));

        return $"INSERT INTO [{table}] ({columnsString}) VALUES ({valuesString})";
    }

    /// <summary>
    /// Checks for presence of <i>INSERT</i>, <i>UPDATE</i>, <i>DELETE</i> or <i>DROP</i> commands in the <paramref name="selectQuery"/>.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public static void ValidateSelectQueryCommandText(string selectQuery)
    {
        foreach (Regex illegalSequenceRegex in s_illegalSequenceRegexes)
        {
            if (illegalSequenceRegex.IsMatch(selectQuery))
            {
                throw new ArgumentException($"Illegal select query: \"{selectQuery}\"");
            }
        }
    }

    /// <exception cref="ArgumentException"></exception>
    public static void ValidateName(string name)
    {
        if (name.Contains(']'))
        {
            throw new ArgumentException($"Illegal sequence in name (']'): '{name}'");
        }
    }

    /// <exception cref="ValueMappingForSqlCommandNotFoundException"></exception>
    public static string MapValueForSqlCommand(dynamic value)
    {
        Func<dynamic, string>? applyMapping = s_predicatesValueMappings
            .Where(mapping => mapping.Key.Invoke(value))
            .Select(mapping => mapping.Value)
            .FirstOrDefault();

        return applyMapping != null
            ? (string)applyMapping.Invoke(value)
            : throw new ValueMappingForSqlCommandNotFoundException(value.GetType().ToString());
    }

    public class ValueMappingForSqlCommandNotFoundException(string message) : Exception(message)
    {
    }

    public class MsSqlException : Exception
    {
        internal MsSqlException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }

    [GeneratedRegex(@"^([^']*'[^']*')*[^']*\s+(INSERT)\s+", RegexOptions.IgnoreCase)]
    private static partial Regex InsertCommandRegex();

    [GeneratedRegex(@"^([^']*'[^']*')*[^']*\s+(UPDATE)\s+", RegexOptions.IgnoreCase)]
    private static partial Regex UpdateCommandRegex();

    [GeneratedRegex(@"^([^']*'[^']*')*[^']*\s+(DELETE)\s+", RegexOptions.IgnoreCase)]
    private static partial Regex DeleteCommandRegex();

    [GeneratedRegex(@"^([^']*'[^']*')*[^']*\s+(DROP)\s+", RegexOptions.IgnoreCase)]
    private static partial Regex DropCommandRegex();
}
