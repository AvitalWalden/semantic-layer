using SemanticLayer.Domain.Enums;

namespace SemanticLayer.Domain.Entities;

/// <summary>
/// An audit record for a single synchronization run (schema introspection or
/// metadata merge), with counts of what changed.
/// </summary>
public class SyncRun
{
    public int Id { get; set; }

    public SyncType Type { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    public int EntitiesAdded { get; set; }
    public int EntitiesRemoved { get; set; }
    public int FieldsAdded { get; set; }
    public int FieldsUpdated { get; set; }
    public int FieldsRemoved { get; set; }

    /// <summary>Human readable summary of the run.</summary>
    public string Summary { get; set; } = string.Empty;
}
