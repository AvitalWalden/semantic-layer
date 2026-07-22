using SemanticLayer.Domain.Enums;

namespace SemanticLayer.Domain.Entities;

/// <summary>
/// A business-facing entity that maps to a single physical table in the source.
/// Holds the business metadata (friendly name, description, visibility) layered
/// on top of the physical table.
/// </summary>
public class SemanticEntity
{
    public int Id { get; set; }

    public int DataSourceId { get; set; }
    public DataSource? DataSource { get; set; }

    /// <summary>Physical table name in the source schema (immutable mapping key).</summary>
    public string PhysicalTableName { get; set; } = string.Empty;

    /// <summary>Business friendly name shown to users.</summary>
    public string BusinessName { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>Whether the entity is exposed to business users.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Active if the physical table still exists; Orphaned otherwise.</summary>
    public ObjectStatus Status { get; set; } = ObjectStatus.Active;

    /// <summary>Primary key column of the physical table (used for stable ordering).</summary>
    public string? PrimaryKeyColumn { get; set; }

    /// <summary>
    /// True once a human edits the entity. Automated syncs (schema/metadata)
    /// never overwrite user-modified records, guaranteeing precedence:
    /// user edits &gt; metadata file &gt; schema defaults.
    /// </summary>
    public bool IsUserModified { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SemanticField> Fields { get; set; } = new List<SemanticField>();
}
