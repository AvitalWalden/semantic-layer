using Microsoft.Extensions.Options;
using Npgsql;
using SemanticLayer.Application.Abstractions;
using SemanticLayer.Application.Configuration;
using SemanticLayer.Application.Dtos;
using SemanticLayer.Domain.Entities;
using SemanticLayer.Domain.Enums;
using SemanticLayer.Infrastructure.Common;

namespace SemanticLayer.Infrastructure.Data;

public class PostgresDataQueryService : IDataQueryService
{
    private readonly ISemanticRepository _repo;
    private readonly NpgsqlDataSource _dataSource;
    private readonly SemanticLayerOptions _options;

    public PostgresDataQueryService(
        ISemanticRepository repo,
        NpgsqlDataSource dataSource,
        IOptions<SemanticLayerOptions> options)
    {
        _repo = repo;
        _dataSource = dataSource;
        _options = options.Value;
    }

    public async Task<DataResultDto> GetDataAsync(int entityId, int page, int pageSize, CancellationToken ct = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 200 ? 25 : pageSize;

        var entity = await _repo.GetEntityAsync(entityId, includeFields: true, ct)
            ?? throw new KeyNotFoundException($"Entity {entityId} not found.");

        if (entity.Status == ObjectStatus.Orphaned)
            throw new InvalidOperationException("The underlying table no longer exists (entity is orphaned).");

        var schema = _options.SourceSchema;
        var table = entity.PhysicalTableName;

        var fields = entity.Fields
            .Where(f => f.IsVisible && f.Status == ObjectStatus.Active)
            .OrderBy(f => f.SortOrder)
            .ToList();

        var columns = fields
            .Select(f => new DataColumnDto(f.BusinessName, f.Unit, f.IsDerived))
            .ToList();

        if (fields.Count == 0)
        {
            return new DataResultDto(entity.Id, entity.BusinessName, columns,
                new List<IDictionary<string, object?>>(), page, pageSize, 0);
        }

        var qualifiedTable = $"{SqlIdentifier.Quote(schema)}.{SqlIdentifier.Quote(table)}";
        var selectList = string.Join(", ", fields.Select(BuildSelectExpression));
        var orderBy = BuildOrderBy(entity, fields);

        var dataSql = $"SELECT {selectList} FROM {qualifiedTable} {orderBy} LIMIT @limit OFFSET @offset;";
        var countSql = $"SELECT COUNT(*) FROM {qualifiedTable};";

        await using var conn = await _dataSource.OpenConnectionAsync(ct);

        long total;
        await using (var countCmd = new NpgsqlCommand(countSql, conn))
        {
            total = Convert.ToInt64(await countCmd.ExecuteScalarAsync(ct));
        }

        var rows = new List<IDictionary<string, object?>>();
        await using (var cmd = new NpgsqlCommand(dataSql, conn))
        {
            cmd.Parameters.AddWithValue("limit", pageSize);
            cmd.Parameters.AddWithValue("offset", (page - 1) * pageSize);

            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                var row = new Dictionary<string, object?>();
                for (var i = 0; i < fields.Count; i++)
                {
                    var key = DedupeKey(row, fields[i].BusinessName);
                    var value = await reader.IsDBNullAsync(i, ct) ? null : reader.GetValue(i);
                    row[key] = value;
                }
                rows.Add(row);
            }
        }

        return new DataResultDto(entity.Id, entity.BusinessName, columns, rows, page, pageSize, total);
    }

    private static string BuildSelectExpression(SemanticField field)
    {
        if (field.IsDerived)
        {
            var expr = field.DerivedExpression ?? string.Empty;
            if (expr.Contains(';'))
                throw new InvalidOperationException($"Derived expression for '{field.PhysicalColumnName}' is not allowed.");
            return $"({expr}) AS {SqlIdentifier.Quote(field.PhysicalColumnName)}";
        }

        return SqlIdentifier.Quote(field.PhysicalColumnName);
    }

    private static string BuildOrderBy(SemanticEntity entity, IReadOnlyList<SemanticField> fields)
    {
        if (!string.IsNullOrWhiteSpace(entity.PrimaryKeyColumn) && SqlIdentifier.IsValid(entity.PrimaryKeyColumn))
            return $"ORDER BY {SqlIdentifier.Quote(entity.PrimaryKeyColumn!)}";

        var firstPhysical = fields.FirstOrDefault(f => !f.IsDerived);
        return firstPhysical is not null
            ? $"ORDER BY {SqlIdentifier.Quote(firstPhysical.PhysicalColumnName)}"
            : string.Empty;
    }

    private static string DedupeKey(IDictionary<string, object?> row, string baseKey)
    {
        if (!row.ContainsKey(baseKey)) return baseKey;
        var i = 2;
        while (row.ContainsKey($"{baseKey} ({i})")) i++;
        return $"{baseKey} ({i})";
    }
}
