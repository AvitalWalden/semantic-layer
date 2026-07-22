using SemanticLayer.Domain.Enums;

namespace SemanticLayer.Domain.Entities;

/// <summary>
/// A business-facing field belonging to a <see cref="SemanticEntity"/>.
/// It either maps to a physical column, or is a derived (calculated) field that
/// does not exist in the database and is defined by a SQL expression coming from
/// the external metadata file.
/// </summary>
public class SemanticField
{
    public int Id { get; set; }

    public int EntityId { get; set; }
    public SemanticEntity? Entity { get; set; }

    /// <summary>
    /// For mapped fields: the physical column name.
    /// For derived fields: the logical field name (mapping key within the entity).
    /// </summary>
    public string PhysicalColumnName { get; set; } = string.Empty;

    public string BusinessName { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>Physical data type reported by the source (null for derived fields until set).</summary>
    public string? PhysicalDataType { get; set; }

    public bool IsVisible { get; set; } = true;

    // ---- Attributes enriched from the external metadata file (not in the DB) ----

    /// <summary>Marks personally identifiable information.</summary>
    public bool IsPii { get; set; }

    public SensitivityLevel SensitivityLevel { get; set; } = SensitivityLevel.Internal;

    /// <summary>Unit of measure, e.g. "USD", "years".</summary>
    public string? Unit { get; set; }

    /// <summary>Optional display/format hint for the UI.</summary>
    public string? DisplayFormat { get; set; }

    // ---- Derived (calculated) field support ----

    /// <summary>True when this field is calculated and has no backing physical column.</summary>
    public bool IsDerived { get; set; }

    /// <summary>
    /// SQL expression evaluated against the entity's physical table (columns of the
    /// same table only). Example: "first_name || ' ' || last_name".
    /// </summary>
    public string? DerivedExpression { get; set; }

    public ObjectStatus Status { get; set; } = ObjectStatus.Active;

    public int SortOrder { get; set; }

    /// <summary>True once a human edits the field; protects it from being overwritten by syncs.</summary>
    public bool IsUserModified { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
