namespace SemanticLayer.Domain.Enums;

/// <summary>
/// Lifecycle status of a semantic entity or field relative to the physical source.
/// </summary>
public enum ObjectStatus
{
    /// <summary>The mapped physical table/column still exists in the source.</summary>
    Active = 0,

    /// <summary>
    /// The mapped physical table/column no longer exists in the source.
    /// The semantic record is kept (not deleted) to preserve business edits.
    /// </summary>
    Orphaned = 1
}
