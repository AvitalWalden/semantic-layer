using SemanticLayer.Domain.Enums;

namespace SemanticLayer.Application.Dtos;

/// <summary>Summary view of a semantic entity (no fields).</summary>
public record EntityDto(
    int Id,
    string PhysicalTableName,
    string BusinessName,
    string? Description,
    bool IsVisible,
    ObjectStatus Status,
    bool IsUserModified,
    int FieldCount);

/// <summary>Full view of a semantic entity including its fields.</summary>
public record EntityDetailDto(
    int Id,
    string PhysicalTableName,
    string BusinessName,
    string? Description,
    bool IsVisible,
    ObjectStatus Status,
    string? PrimaryKeyColumn,
    bool IsUserModified,
    IReadOnlyList<FieldDto> Fields);

/// <summary>View of a semantic field.</summary>
public record FieldDto(
    int Id,
    string PhysicalColumnName,
    string BusinessName,
    string? Description,
    string? PhysicalDataType,
    bool IsVisible,
    bool IsPii,
    SensitivityLevel SensitivityLevel,
    string? Unit,
    string? DisplayFormat,
    bool IsDerived,
    string? DerivedExpression,
    ObjectStatus Status,
    int SortOrder,
    bool IsUserModified);

/// <summary>Editable business attributes of an entity.</summary>
public record UpdateEntityDto(
    string BusinessName,
    string? Description,
    bool IsVisible);

/// <summary>Editable business attributes of a field.</summary>
public record UpdateFieldDto(
    string BusinessName,
    string? Description,
    bool IsVisible,
    bool IsPii,
    SensitivityLevel SensitivityLevel,
    string? Unit,
    string? DisplayFormat);
