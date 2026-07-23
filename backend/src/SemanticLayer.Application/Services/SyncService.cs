using Microsoft.Extensions.Options;
using SemanticLayer.Application.Abstractions;
using SemanticLayer.Application.Common;
using SemanticLayer.Application.Configuration;
using SemanticLayer.Application.Dtos;
using SemanticLayer.Application.Introspection;
using SemanticLayer.Domain.Entities;
using SemanticLayer.Domain.Enums;

namespace SemanticLayer.Application.Services;

public class SyncService : ISyncService
{
    private readonly ISemanticRepository _repo;
    private readonly ISchemaIntrospector _introspector;
    private readonly SemanticLayerOptions _options;

    public SyncService(
        ISemanticRepository repo,
        ISchemaIntrospector introspector,
        IOptions<SemanticLayerOptions> options)
    {
        _repo = repo;
        _introspector = introspector;
        _options = options.Value;
    }

    public async Task<SyncResultDto> SyncSchemaAsync(CancellationToken ct = default)
    {
        var startedAt = DateTime.UtcNow;
        var dataSource = await _repo.GetOrCreateDefaultDataSourceAsync(
            _options.DataSourceName, _options.SourceSchema, ct);

        var physicalTables = await _introspector.GetTablesAsync(_options.SourceSchema, ct);
        var existingEntities = await _repo.GetEntitiesAsync(includeFields: true, onlyVisible: false, ct);

        var byPhysicalName = existingEntities
            .ToDictionary(e => e.PhysicalTableName, StringComparer.OrdinalIgnoreCase);

        int entitiesAdded = 0, entitiesRemoved = 0, fieldsAdded = 0, fieldsUpdated = 0, fieldsRemoved = 0;
        var physicalTableNames = new HashSet<string>(physicalTables.Select(t => t.Name), StringComparer.OrdinalIgnoreCase);

        foreach (var table in physicalTables)
        {
            var pkColumn = table.Columns.FirstOrDefault(c => c.IsPrimaryKey)?.Name;

            if (!byPhysicalName.TryGetValue(table.Name, out var entity))
            {
                entity = new SemanticEntity
                {
                    DataSourceId = dataSource.Id,
                    PhysicalTableName = table.Name,
                    BusinessName = NameHumanizer.Humanize(table.Name),
                    IsVisible = true,
                    Status = ObjectStatus.Active,
                    PrimaryKeyColumn = pkColumn
                };

                foreach (var col in table.Columns.OrderBy(c => c.OrdinalPosition))
                {
                    entity.Fields.Add(CreateFieldFromColumn(col));
                    fieldsAdded++;
                }

                _repo.AddEntity(entity);
                entitiesAdded++;
                continue;
            }

            entity.Status = ObjectStatus.Active;
            entity.PrimaryKeyColumn = pkColumn;
            entity.UpdatedAt = DateTime.UtcNow;

            var (added, updated, removed) = ReconcileFields(entity, table);
            fieldsAdded += added;
            fieldsUpdated += updated;
            fieldsRemoved += removed;
        }

        foreach (var entity in existingEntities)
        {
            if (!physicalTableNames.Contains(entity.PhysicalTableName) && entity.Status != ObjectStatus.Orphaned)
            {
                entity.Status = ObjectStatus.Orphaned;
                entity.UpdatedAt = DateTime.UtcNow;
                entitiesRemoved++;
            }
        }

        var summary =
            $"Schema sync against '{_options.SourceSchema}': " +
            $"+{entitiesAdded} entities, {entitiesRemoved} orphaned; " +
            $"fields +{fieldsAdded}/~{fieldsUpdated}/-{fieldsRemoved}.";

        var run = new SyncRun
        {
            Type = SyncType.Schema,
            StartedAt = startedAt,
            CompletedAt = DateTime.UtcNow,
            EntitiesAdded = entitiesAdded,
            EntitiesRemoved = entitiesRemoved,
            FieldsAdded = fieldsAdded,
            FieldsUpdated = fieldsUpdated,
            FieldsRemoved = fieldsRemoved,
            Summary = summary
        };
        await _repo.AddSyncRunAsync(run, ct);
        await _repo.SaveChangesAsync(ct);

        return new SyncResultDto(SyncType.Schema, entitiesAdded, entitiesRemoved, fieldsAdded, fieldsUpdated, fieldsRemoved, summary);
    }

    private static (int added, int updated, int removed) ReconcileFields(SemanticEntity entity, PhysicalTable table)
    {
        int added = 0, updated = 0, removed = 0;

        var mappedFields = entity.Fields
            .Where(f => !f.IsDerived)
            .ToDictionary(f => f.PhysicalColumnName, StringComparer.OrdinalIgnoreCase);

        var physicalColumnNames = new HashSet<string>(table.Columns.Select(c => c.Name), StringComparer.OrdinalIgnoreCase);

        foreach (var col in table.Columns.OrderBy(c => c.OrdinalPosition))
        {
            if (!mappedFields.TryGetValue(col.Name, out var field))
            {
                var newField = CreateFieldFromColumn(col);
                newField.EntityId = entity.Id;
                entity.Fields.Add(newField);
                added++;
                continue;
            }

            field.Status = ObjectStatus.Active;
            field.SortOrder = col.OrdinalPosition;

            if (!string.Equals(field.PhysicalDataType, col.DataType, StringComparison.OrdinalIgnoreCase))
            {
                field.PhysicalDataType = col.DataType;
                field.UpdatedAt = DateTime.UtcNow;
                updated++;
            }
        }

        foreach (var field in entity.Fields.Where(f => !f.IsDerived))
        {
            if (!physicalColumnNames.Contains(field.PhysicalColumnName) && field.Status != ObjectStatus.Orphaned)
            {
                field.Status = ObjectStatus.Orphaned;
                field.UpdatedAt = DateTime.UtcNow;
                removed++;
            }
        }

        return (added, updated, removed);
    }

    private static SemanticField CreateFieldFromColumn(PhysicalColumn col) => new()
    {
        PhysicalColumnName = col.Name,
        BusinessName = NameHumanizer.Humanize(col.Name),
        PhysicalDataType = col.DataType,
        IsVisible = true,
        Status = ObjectStatus.Active,
        SortOrder = col.OrdinalPosition
    };
}
