namespace SemanticLayer.Domain.Entities;

/// <summary>
/// A connected relational data source that the semantic layer is built on top of.
/// This demo works with a single default data source, but the model supports many.
/// </summary>
public class DataSource
{
    public int Id { get; set; }

    /// <summary>Human friendly name of the source, e.g. "HR Database".</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>The physical schema that is introspected, e.g. "hr".</summary>
    public string SourceSchema { get; set; } = string.Empty;

    /// <summary>Optional free-text note about the source (not the secret connection string).</summary>
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SemanticEntity> Entities { get; set; } = new List<SemanticEntity>();
}
