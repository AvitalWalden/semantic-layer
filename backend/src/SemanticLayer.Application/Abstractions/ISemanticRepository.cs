using SemanticLayer.Domain.Entities;

namespace SemanticLayer.Application.Abstractions;

/// <summary>
/// Persistence gateway for the semantic metadata store. Implemented with EF Core
/// in the Infrastructure layer.
/// </summary>
public interface ISemanticRepository
{
    Task<DataSource> GetOrCreateDefaultDataSourceAsync(string name, string schema, CancellationToken ct = default);

    Task<IReadOnlyList<SemanticEntity>> GetEntitiesAsync(bool includeFields, bool onlyVisible, CancellationToken ct = default);

    Task<SemanticEntity?> GetEntityAsync(int id, bool includeFields, CancellationToken ct = default);

    Task<SemanticField?> GetFieldAsync(int id, CancellationToken ct = default);

    void AddEntity(SemanticEntity entity);

    void AddField(SemanticField field);

    Task AddSyncRunAsync(SyncRun run, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
