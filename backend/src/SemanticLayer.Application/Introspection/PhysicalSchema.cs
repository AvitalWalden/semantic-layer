namespace SemanticLayer.Application.Introspection;

public record PhysicalColumn(
    string Name,
    string DataType,
    bool IsNullable,
    bool IsPrimaryKey,
    int OrdinalPosition);

public record PhysicalTable(
    string Name,
    IReadOnlyList<PhysicalColumn> Columns);
