using SemanticLayer.Domain.Enums;

namespace SemanticLayer.Domain.Entities;

public class SemanticEntity
{
    public int Id { get; set; }
    public int DataSourceId { get; set; }
    public DataSource? DataSource { get; set; }
    public string PhysicalTableName { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsVisible { get; set; } = true;
    public ObjectStatus Status { get; set; } = ObjectStatus.Active;
    public string? PrimaryKeyColumn { get; set; }
    public bool IsUserModified { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SemanticField> Fields { get; set; } = new List<SemanticField>();
}
