using SemanticLayer.Application.Dtos;

namespace SemanticLayer.Application.Abstractions;

public interface ISemanticService
{
    Task<IReadOnlyList<EntityDto>> GetEntitiesAsync(bool onlyVisible, CancellationToken ct = default);
    Task<EntityDetailDto?> GetEntityAsync(int id, CancellationToken ct = default);
    Task<EntityDetailDto?> UpdateEntityAsync(int id, UpdateEntityDto dto, CancellationToken ct = default);
    Task<FieldDto?> UpdateFieldAsync(int id, UpdateFieldDto dto, CancellationToken ct = default);
}

public interface ISyncService
{
    Task<SyncResultDto> SyncSchemaAsync(CancellationToken ct = default);
}

public interface IMetadataMergeService
{
    Task<SyncResultDto> MergeAsync(Stream metadataJson, CancellationToken ct = default);
}

public interface IDataQueryService
{
    Task<DataResultDto> GetDataAsync(int entityId, int page, int pageSize, CancellationToken ct = default);
}
