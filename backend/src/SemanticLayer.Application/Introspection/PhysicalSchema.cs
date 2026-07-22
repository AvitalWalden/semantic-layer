namespace SemanticLayer.Application.Introspection;

/// <summary>A physical column as read from the source via introspection.</summary>
public record PhysicalColumn(
    string Name,
    string DataType,
    bool IsNullable,
    bool IsPrimaryKey,
    int OrdinalPosition);

/// <summary>A physical table with its columns, as read from the source via introspection.</summary>
public record PhysicalTable(
    string Name,
    IReadOnlyList<PhysicalColumn> Columns);
