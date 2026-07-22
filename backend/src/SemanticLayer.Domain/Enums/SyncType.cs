namespace SemanticLayer.Domain.Enums;

/// <summary>Kind of synchronization run recorded in the sync history.</summary>
public enum SyncType
{
    /// <summary>Structural sync: read the physical schema via introspection.</summary>
    Schema = 0,

    /// <summary>Enrichment sync: merge an external metadata file.</summary>
    Metadata = 1
}
