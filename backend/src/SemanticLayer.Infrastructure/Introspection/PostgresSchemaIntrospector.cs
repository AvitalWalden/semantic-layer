using Npgsql;
using SemanticLayer.Application.Abstractions;
using SemanticLayer.Application.Introspection;

namespace SemanticLayer.Infrastructure.Introspection;

/// <summary>
/// Reads table and column structure from a PostgreSQL schema dynamically using
/// the standard information_schema plus pg_index for primary-key detection.
/// </summary>
public class PostgresSchemaIntrospector : ISchemaIntrospector
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresSchemaIntrospector(NpgsqlDataSource dataSource) => _dataSource = dataSource;

    public async Task<IReadOnlyList<PhysicalTable>> GetTablesAsync(string schema, CancellationToken ct = default)
    {
        var primaryKeys = await GetPrimaryKeyColumnsAsync(schema, ct);
        var columnsByTable = new Dictionary<string, List<PhysicalColumn>>(StringComparer.Ordinal);

        const string sql = @"
            SELECT c.table_name,
                   c.column_name,
                   c.data_type,
                   c.is_nullable,
                   c.ordinal_position
            FROM information_schema.columns c
            JOIN information_schema.tables t
              ON t.table_schema = c.table_schema
             AND t.table_name   = c.table_name
            WHERE c.table_schema = @schema
              AND t.table_type = 'BASE TABLE'
            ORDER BY c.table_name, c.ordinal_position;";

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("schema", schema);

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var tableName = reader.GetString(0);
            var columnName = reader.GetString(1);
            var dataType = reader.GetString(2);
            var isNullable = string.Equals(reader.GetString(3), "YES", StringComparison.OrdinalIgnoreCase);
            var ordinal = reader.GetInt32(4);

            var isPk = primaryKeys.TryGetValue(tableName, out var pkCols) && pkCols.Contains(columnName);

            if (!columnsByTable.TryGetValue(tableName, out var list))
            {
                list = new List<PhysicalColumn>();
                columnsByTable[tableName] = list;
            }
            list.Add(new PhysicalColumn(columnName, dataType, isNullable, isPk, ordinal));
        }

        return columnsByTable
            .Select(kv => new PhysicalTable(kv.Key, kv.Value))
            .OrderBy(t => t.Name)
            .ToList();
    }

    private async Task<Dictionary<string, HashSet<string>>> GetPrimaryKeyColumnsAsync(string schema, CancellationToken ct)
    {
        const string sql = @"
            SELECT c.relname AS table_name, a.attname AS column_name
            FROM pg_index i
            JOIN pg_class c     ON c.oid = i.indrelid
            JOIN pg_namespace n ON n.oid = c.relnamespace
            JOIN pg_attribute a ON a.attrelid = c.oid AND a.attnum = ANY (i.indkey)
            WHERE i.indisprimary
              AND n.nspname = @schema;";

        var result = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("schema", schema);

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var table = reader.GetString(0);
            var column = reader.GetString(1);
            if (!result.TryGetValue(table, out var set))
            {
                set = new HashSet<string>(StringComparer.Ordinal);
                result[table] = set;
            }
            set.Add(column);
        }

        return result;
    }
}
