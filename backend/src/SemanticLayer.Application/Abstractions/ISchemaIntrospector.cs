using SemanticLayer.Application.Introspection;

namespace SemanticLayer.Application.Abstractions;


public interface ISchemaIntrospector
{
    Task<IReadOnlyList<PhysicalTable>> GetTablesAsync(string schema, CancellationToken ct = default);
}
