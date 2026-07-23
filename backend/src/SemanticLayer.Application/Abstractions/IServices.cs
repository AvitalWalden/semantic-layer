using SemanticLayer.Application.Dtos;

namespace SemanticLayer.Application.Abstractions;

/// <summary>Reads and updates the business metadata of the semantic layer.</summary>
public interface ISemanticService
{
    Task<IReadOnlyList<EntityDto>> GetEntitiesAsync(bool onlyVisible, CancellationToken ct = default);
    Task<EntityDetailDto?> GetEntityAsync(int id, CancellationToken ct = default);
    Task<EntityDetailDto?> UpdateEntityAsync(int id, UpdateEntityDto dto, CancellationToken ct = default);
    Task<FieldDto?> UpdateFieldAsync(int id, UpdateFieldDto dto, CancellationToken ct = default);
}

/// <summary>Synchronizes the semantic layer with the physical schema (introspection).</summary>
public interface ISyncService
{
    Task<SyncResultDto> SyncSchemaAsync(CancellationToken ct = default);
}

/// <summary>Merges an external metadata file into the semantic layer.</summary>
public interface IMetadataMergeService
{
    Task<SyncResultDto> MergeAsync(Stream metadataJson, CancellationToken ct = default);
}

/// <summary>Queries source data through the semantic layer (visible fields only).</summary>
public interface IDataQueryService
{
    Task<DataResultDto> GetDataAsync(int entityId, int page, int pageSize, CancellationToken ct = default);
}
