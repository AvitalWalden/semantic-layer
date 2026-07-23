using SemanticLayer.Domain.Enums;

namespace SemanticLayer.Application.Dtos;

public record EntityDto(
    int Id,
    string PhysicalTableName,
    string BusinessName,
    string? Description,
    bool IsVisible,
    ObjectStatus Status,
    bool IsUserModified,
    int FieldCount);

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

public record UpdateEntityDto(
    string BusinessName,
    string? Description,
    bool IsVisible);

public record UpdateFieldDto(
    string BusinessName,
    string? Description,
    bool IsVisible,
    bool IsPii,
    SensitivityLevel SensitivityLevel,
    string? Unit,
    string? DisplayFormat);
