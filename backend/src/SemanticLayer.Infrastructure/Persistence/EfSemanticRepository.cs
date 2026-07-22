using Microsoft.EntityFrameworkCore;
using SemanticLayer.Application.Abstractions;
using SemanticLayer.Domain.Entities;

namespace SemanticLayer.Infrastructure.Persistence;

public class EfSemanticRepository : ISemanticRepository
{
    private readonly SemanticDbContext _db;

    public EfSemanticRepository(SemanticDbContext db) => _db = db;

    public async Task<DataSource> GetOrCreateDefaultDataSourceAsync(string name, string schema, CancellationToken ct = default)
    {
        var existing = await _db.DataSources.FirstOrDefaultAsync(ct);
        if (existing is not null)
            return existing;

        var ds = new DataSource { Name = name, SourceSchema = schema };
        _db.DataSources.Add(ds);
        await _db.SaveChangesAsync(ct);
        return ds;
    }

    public async Task<IReadOnlyList<SemanticEntity>> GetEntitiesAsync(bool includeFields, bool onlyVisible, CancellationToken ct = default)
    {
        IQueryable<SemanticEntity> query = _db.Entities;
        if (includeFields)
            query = query.Include(e => e.Fields);
        if (onlyVisible)
            query = query.Where(e => e.IsVisible && e.Status == Domain.Enums.ObjectStatus.Active);

        return await query.OrderBy(e => e.BusinessName).ToListAsync(ct);
    }

    public async Task<SemanticEntity?> GetEntityAsync(int id, bool includeFields, CancellationToken ct = default)
    {
        IQueryable<SemanticEntity> query = _db.Entities;
        if (includeFields)
            query = query.Include(e => e.Fields);
        return await query.FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public Task<SemanticField?> GetFieldAsync(int id, CancellationToken ct = default) =>
        _db.Fields.FirstOrDefaultAsync(f => f.Id == id, ct);

    public void AddEntity(SemanticEntity entity) => _db.Entities.Add(entity);

    public void AddField(SemanticField field) => _db.Fields.Add(field);

    public async Task AddSyncRunAsync(SyncRun run, CancellationToken ct = default)
    {
        await _db.SyncRuns.AddAsync(run, ct);
    }

    public async Task<IReadOnlyList<SyncRun>> GetSyncRunsAsync(int take, CancellationToken ct = default) =>
        await _db.SyncRuns.OrderByDescending(r => r.StartedAt).Take(take).ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
