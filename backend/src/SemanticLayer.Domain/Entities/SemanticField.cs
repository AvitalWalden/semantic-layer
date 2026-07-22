using SemanticLayer.Domain.Enums;

namespace SemanticLayer.Domain.Entities;

public class SemanticField
{
    public int Id { get; set; }

    public int EntityId { get; set; }
    public SemanticEntity? Entity { get; set; }
    public string PhysicalColumnName { get; set; } = string.Empty;

    public string BusinessName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? PhysicalDataType { get; set; }

    public bool IsVisible { get; set; } = true;

    public bool IsPii { get; set; }

    public SensitivityLevel SensitivityLevel { get; set; } = SensitivityLevel.Internal;

    public string? Unit { get; set; }

    public string? DisplayFormat { get; set; }

    public bool IsDerived { get; set; }
    public string? DerivedExpression { get; set; }

    public ObjectStatus Status { get; set; } = ObjectStatus.Active;

    public int SortOrder { get; set; }
    public bool IsUserModified { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
