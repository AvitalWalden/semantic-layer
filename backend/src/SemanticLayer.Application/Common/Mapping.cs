using SemanticLayer.Application.Dtos;
using SemanticLayer.Domain.Entities;

namespace SemanticLayer.Application.Common;

/// <summary>Maps domain entities to DTOs.</summary>
public static class Mapping
{
    public static EntityDto ToDto(this SemanticEntity e) => new(
        e.Id,
        e.PhysicalTableName,
        e.BusinessName,
        e.Description,
        e.IsVisible,
        e.Status,
        e.IsUserModified,
        e.Fields?.Count ?? 0);

    public static EntityDetailDto ToDetailDto(this SemanticEntity e) => new(
        e.Id,
        e.PhysicalTableName,
        e.BusinessName,
        e.Description,
        e.IsVisible,
        e.Status,
        e.PrimaryKeyColumn,
        e.IsUserModified,
        (e.Fields ?? new List<SemanticField>())
            .OrderBy(f => f.SortOrder)
            .Select(ToDto)
            .ToList());

    public static FieldDto ToDto(this SemanticField f) => new(
        f.Id,
        f.PhysicalColumnName,
        f.BusinessName,
        f.Description,
        f.PhysicalDataType,
        f.IsVisible,
        f.IsPii,
        f.SensitivityLevel,
        f.Unit,
        f.DisplayFormat,
        f.IsDerived,
        f.DerivedExpression,
        f.Status,
        f.SortOrder,
        f.IsUserModified);
}
