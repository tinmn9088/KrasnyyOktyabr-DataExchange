using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MsSql.Extensions;

namespace MsSql;

public class MsSqlService(ILogger<MsSqlService> logger) : IMsSqlService
{
    private static readonly Regex s_insertCommandRegex = new(@"^([^']*'[^']*')*[^']*\s*(INSERT)\s*");

    private static readonly Regex s_updateCommandRegex = new(@"^([^']*'[^']*')*[^']*\s*(UPDATE)\s*");

    private static readonly Regex s_deleteCommandRegex = new(@"^([^']*'[^']*')*[^']*\s*(DELETE)\s*");

    private static readonly Regex s_dropCommandRegex = new(@"^([^']*'[^']*')*[^']*\s*(DROP)\s*");

    private static readonly Regex[] s_illegalSequenceRegexes = [s_insertCommandRegex, s_updateCommandRegex, s_deleteCommandRegex, s_dropCommandRegex];

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

#nullable enable
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="MsSqlException"></exception>
    public async Task<object?> SelectSingleValueAsync(string connectionString, string query)
    {
        return await SelectSingleValueAsync(connectionString, query, IMsSqlService.ConnectionType.OleDbConnection);
    }

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="MsSqlException"></exception>
    public async Task<object?> SelectSingleValueAsync(string connectionString, string query, IMsSqlService.ConnectionType connectionType)
    {
        ValidateSelectQueryCommandText(query);

        try
        {
            logger.LogConnecting(connectionType);

            IDbConnectionFactory factory = DbConnectionAbstractFactory.GetConnectionFactory(connectionType);

            using DbConnection connection = factory.CreateConnection(connectionString);

            await connection.OpenAsync().ConfigureAwait(false);

            using DbCommand command = factory.CreateCommand(query, connection);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            using DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            return reader.HasRows && reader.Read()
                ? reader.GetValue(0)
                : null;
        }
        catch (Exception ex)
        {
            throw new MsSqlException(ex.Message, ex);
        }
    }

#nullable disable
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="MsSqlException"></exception>
    public async Task InsertAsync(string connectionString, string table, Dictionary<string, dynamic> columnsValues)
    {
        await InsertAsync(connectionString, table, columnsValues, IMsSqlService.ConnectionType.OleDbConnection);
    }

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="MsSqlException"></exception>
    public async Task InsertAsync(string connectionString, string table, Dictionary<string, dynamic> columnsValues, IMsSqlService.ConnectionType connectionType)
    {
        string commandText = BuildInsertQueryText(table, columnsValues);

        try
        {
            logger.LogConnecting(connectionType);

            IDbConnectionFactory factory = DbConnectionAbstractFactory.GetConnectionFactory(connectionType);

            using DbConnection connection = factory.CreateConnection(connectionString);

            await connection.OpenAsync().ConfigureAwait(false);

            using DbCommand command = factory.CreateCommand(commandText, connection);

            int insertedRowsCount = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            logger.LogTrace("{InsertedRowsCount} rows inserted", insertedRowsCount);
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
                throw new ArgumentException($"Illegal select query: '{selectQuery}'");
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
#nullable enable
        Func<dynamic, string>? applyMapping = s_predicatesValueMappings
            .Where(mapping => mapping.Key.Invoke(value))
            .Select(mapping => mapping.Value)
            .FirstOrDefault();
#nullable disable

        return applyMapping is not null
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

    private class DbConnectionAbstractFactory
    {
        internal static IDbConnectionFactory GetConnectionFactory(IMsSqlService.ConnectionType connectionType)
        {
            return connectionType switch
            {
                IMsSqlService.ConnectionType.SqlConnection => new SqlConnectionFactory(),
                IMsSqlService.ConnectionType.OleDbConnection => new OleDbConnectionFactory(),
                _ => throw new NotImplementedException(),
            };
        }
    }

    private interface IDbConnectionFactory
    {
        DbConnection CreateConnection(string connectionString);

        DbCommand CreateCommand(string commandText, DbConnection connection);
    }

    private class SqlConnectionFactory : IDbConnectionFactory
    {
        public DbConnection CreateConnection(string connectionString) => new SqlConnection(connectionString);

        public DbCommand CreateCommand(string commandText, DbConnection connection)
        {
            if (connection is SqlConnection sqlConnection)
            {
                SqlCommand command = new(commandText, sqlConnection)
                {
                    CommandTimeout = 0, // Disable timeout
                };

                return command;
            }

            throw new NotImplementedException();
        }
    }

    private class OleDbConnectionFactory : IDbConnectionFactory
    {
        public DbConnection CreateConnection(string connectionString) => new OleDbConnection(connectionString);

        public DbCommand CreateCommand(string commandText, DbConnection connection)
        {
            if (connection is OleDbConnection sqlConnection)
            {
                OleDbCommand command = new(commandText, sqlConnection)
                {
                    CommandTimeout = 0, // Disable timeout
                };

                return command;
            }

            throw new NotImplementedException();
        }
    }
}
