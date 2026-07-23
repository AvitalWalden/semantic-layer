namespace SemanticLayer.Application.Dtos;

public record DataColumnDto(
    string BusinessName,
    string? Unit,
    bool IsDerived);

public record DataResultDto(
    int EntityId,
    string EntityBusinessName,
    IReadOnlyList<DataColumnDto> Columns,
    IReadOnlyList<IDictionary<string, object?>> Rows,
    int Page,
    int PageSize,
    long TotalRows);
