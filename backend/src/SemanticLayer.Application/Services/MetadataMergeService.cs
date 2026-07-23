using System.Text.Json;
using SemanticLayer.Application.Abstractions;
using SemanticLayer.Application.Dtos;
using SemanticLayer.Application.Metadata;
using SemanticLayer.Domain.Entities;
using SemanticLayer.Domain.Enums;

namespace SemanticLayer.Application.Services;

public class MetadataMergeService : IMetadataMergeService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ISemanticRepository _repo;

    public MetadataMergeService(ISemanticRepository repo) => _repo = repo;

    public async Task<SyncResultDto> MergeAsync(Stream metadataJson, CancellationToken ct = default)
    {
        var startedAt = DateTime.UtcNow;

        MetadataFile? file;
        try
        {
            file = await JsonSerializer.DeserializeAsync<MetadataFile>(metadataJson, JsonOptions, ct);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"Invalid metadata file: {ex.Message}", ex);
        }

        if (file is null)
            throw new InvalidDataException("Metadata file is empty or could not be parsed.");

        var entities = await _repo.GetEntitiesAsync(includeFields: true, onlyVisible: false, ct);
        var entitiesByName = entities.ToDictionary(e => e.PhysicalTableName, StringComparer.OrdinalIgnoreCase);

        int fieldsAdded = 0, fieldsUpdated = 0, entitiesUpdated = 0, skipped = 0;

        foreach (var (tableName, tableMeta) in file.Tables)
        {
            if (!entitiesByName.TryGetValue(tableName, out var entity))
            {
                skipped++;
                continue; 
            }

            if (!entity.IsUserModified)
            {
                var changed = false;
                if (!string.IsNullOrWhiteSpace(tableMeta.BusinessName))
                {
                    entity.BusinessName = tableMeta.BusinessName!;
                    changed = true;
                }
                if (!string.IsNullOrWhiteSpace(tableMeta.Description))
                {
                    entity.Description = tableMeta.Description;
                    changed = true;
                }
                if (changed)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                    entitiesUpdated++;
                }
            }

            var fieldsByName = entity.Fields
                .Where(f => !f.IsDerived)
                .ToDictionary(f => f.PhysicalColumnName, StringComparer.OrdinalIgnoreCase);

            foreach (var (columnName, columnMeta) in tableMeta.Columns)
            {
                if (!fieldsByName.TryGetValue(columnName, out var field) || field.IsUserModified)
                    continue;

                ApplyColumnMetadata(field, columnMeta);
                field.UpdatedAt = DateTime.UtcNow;
                fieldsUpdated++;
            }

            var derivedByName = entity.Fields
                .Where(f => f.IsDerived)
                .ToDictionary(f => f.PhysicalColumnName, StringComparer.OrdinalIgnoreCase);

            var maxSort = entity.Fields.Count == 0 ? 0 : entity.Fields.Max(f => f.SortOrder);

            foreach (var derived in tableMeta.DerivedFields)
            {
                if (string.IsNullOrWhiteSpace(derived.Name) || string.IsNullOrWhiteSpace(derived.Expression))
                    continue;

                if (derivedByName.TryGetValue(derived.Name, out var existing))
                {
                    if (existing.IsUserModified) continue;
                    ApplyDerivedMetadata(existing, derived);
                    existing.UpdatedAt = DateTime.UtcNow;
                    fieldsUpdated++;
                }
                else
                {
                    var newField = new SemanticField
                    {
                        EntityId = entity.Id,
                        PhysicalColumnName = derived.Name,
                        IsDerived = true,
                        IsVisible = true,
                        Status = ObjectStatus.Active,
                        SortOrder = ++maxSort
                    };
                    ApplyDerivedMetadata(newField, derived);
                    entity.Fields.Add(newField);
                    _repo.AddField(newField);
                    fieldsAdded++;
                }
            }
        }

        var summary =
            $"Metadata merge: entities enriched {entitiesUpdated}, " +
            $"fields +{fieldsAdded}/~{fieldsUpdated}" +
            (skipped > 0 ? $"; {skipped} table(s) in file not found in schema (ignored)." : ".");

        var run = new SyncRun
        {
            Type = SyncType.Metadata,
            StartedAt = startedAt,
            CompletedAt = DateTime.UtcNow,
            EntitiesAdded = 0,
            EntitiesRemoved = 0,
            FieldsAdded = fieldsAdded,
            FieldsUpdated = fieldsUpdated,
            FieldsRemoved = 0,
            Summary = summary
        };
        await _repo.AddSyncRunAsync(run, ct);
        await _repo.SaveChangesAsync(ct);

        return new SyncResultDto(SyncType.Metadata, 0, 0, fieldsAdded, fieldsUpdated, 0, summary);
    }

    private static void ApplyColumnMetadata(SemanticField field, ColumnMetadata meta)
    {
        if (!string.IsNullOrWhiteSpace(meta.BusinessName)) field.BusinessName = meta.BusinessName!;
        if (!string.IsNullOrWhiteSpace(meta.Description)) field.Description = meta.Description;
        if (meta.IsVisible.HasValue) field.IsVisible = meta.IsVisible.Value;
        if (meta.IsPii.HasValue) field.IsPii = meta.IsPii.Value;
        if (meta.SensitivityLevel.HasValue) field.SensitivityLevel = meta.SensitivityLevel.Value;
        if (!string.IsNullOrWhiteSpace(meta.Unit)) field.Unit = meta.Unit;
        if (!string.IsNullOrWhiteSpace(meta.DisplayFormat)) field.DisplayFormat = meta.DisplayFormat;
    }

    private static void ApplyDerivedMetadata(SemanticField field, DerivedFieldMetadata meta)
    {
        field.BusinessName = !string.IsNullOrWhiteSpace(meta.BusinessName)
            ? meta.BusinessName!
            : Common.NameHumanizer.Humanize(meta.Name);
        field.Description = meta.Description ?? field.Description;
        field.DerivedExpression = meta.Expression;
        field.PhysicalDataType = meta.DataType ?? field.PhysicalDataType;
        if (meta.IsPii.HasValue) field.IsPii = meta.IsPii.Value;
        if (meta.SensitivityLevel.HasValue) field.SensitivityLevel = meta.SensitivityLevel.Value;
        if (!string.IsNullOrWhiteSpace(meta.Unit)) field.Unit = meta.Unit;
    }
}
