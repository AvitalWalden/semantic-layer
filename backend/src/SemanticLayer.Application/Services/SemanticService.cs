using SemanticLayer.Application.Abstractions;
using SemanticLayer.Application.Common;
using SemanticLayer.Application.Dtos;

namespace SemanticLayer.Application.Services;

public class SemanticService : ISemanticService
{
    private readonly ISemanticRepository _repo;

    public SemanticService(ISemanticRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<EntityDto>> GetEntitiesAsync(bool onlyVisible, CancellationToken ct = default)
    {
        var entities = await _repo.GetEntitiesAsync(includeFields: true, onlyVisible: onlyVisible, ct);
        return entities.Select(e => e.ToDto()).ToList();
    }

    public async Task<EntityDetailDto?> GetEntityAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetEntityAsync(id, includeFields: true, ct);
        return entity?.ToDetailDto();
    }

    public async Task<EntityDetailDto?> UpdateEntityAsync(int id, UpdateEntityDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetEntityAsync(id, includeFields: true, ct);
        if (entity is null) return null;

        entity.BusinessName = dto.BusinessName;
        entity.Description = dto.Description;
        entity.IsVisible = dto.IsVisible;
        entity.IsUserModified = true; // user edits win over future syncs
        entity.UpdatedAt = DateTime.UtcNow;

        await _repo.SaveChangesAsync(ct);
        return entity.ToDetailDto();
    }

    public async Task<FieldDto?> UpdateFieldAsync(int id, UpdateFieldDto dto, CancellationToken ct = default)
    {
        var field = await _repo.GetFieldAsync(id, ct);
        if (field is null) return null;

        field.BusinessName = dto.BusinessName;
        field.Description = dto.Description;
        field.IsVisible = dto.IsVisible;
        field.IsPii = dto.IsPii;
        field.SensitivityLevel = dto.SensitivityLevel;
        field.Unit = dto.Unit;
        field.DisplayFormat = dto.DisplayFormat;
        field.IsUserModified = true; // user edits win over future syncs
        field.UpdatedAt = DateTime.UtcNow;

        await _repo.SaveChangesAsync(ct);
        return field.ToDto();
    }
}
