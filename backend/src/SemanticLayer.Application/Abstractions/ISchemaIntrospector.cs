using SemanticLayer.Application.Introspection;

namespace SemanticLayer.Application.Abstractions;

/// <summary>
/// Reads the structure of a physical relational schema dynamically.
/// Implemented in the Infrastructure layer against a specific database engine.
/// </summary>
public interface ISchemaIntrospector
{
    Task<IReadOnlyList<PhysicalTable>> GetTablesAsync(string schema, CancellationToken ct = default);
}
