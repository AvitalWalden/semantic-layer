using SemanticLayer.Domain.Enums;

namespace SemanticLayer.Application.Dtos;

public record SyncResultDto(
    SyncType Type,
    int EntitiesAdded,
    int EntitiesRemoved,
    int FieldsAdded,
    int FieldsUpdated,
    int FieldsRemoved,
    string Summary);
