namespace SemanticLayer.Application.Dtos;

/// <summary>A single column descriptor in a data-explorer result (business-facing).</summary>
public record DataColumnDto(
    string BusinessName,
    string? Unit,
    bool IsDerived);

/// <summary>
/// A page of data returned through the semantic layer. Rows are keyed by the
/// business column name; only visible fields are included.
/// </summary>
public record DataResultDto(
    int EntityId,
    string EntityBusinessName,
    IReadOnlyList<DataColumnDto> Columns,
    IReadOnlyList<IDictionary<string, object?>> Rows,
    int Page,
    int PageSize,
    long TotalRows);
