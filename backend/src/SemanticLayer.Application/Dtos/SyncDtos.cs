using SemanticLayer.Domain.Enums;

namespace SemanticLayer.Application.Dtos;

/// <summary>Result of a synchronization run.</summary>
public record SyncResultDto(
    SyncType Type,
    int EntitiesAdded,
    int EntitiesRemoved,
    int FieldsAdded,
    int FieldsUpdated,
    int FieldsRemoved,
    string Summary);

/// <summary>A historical synchronization run.</summary>
public record SyncRunDto(
    int Id,
    SyncType Type,
    DateTime StartedAt,
    DateTime CompletedAt,
    int EntitiesAdded,
    int EntitiesRemoved,
    int FieldsAdded,
    int FieldsUpdated,
    int FieldsRemoved,
    string Summary);
