namespace XTrendApp.Web.Models.Entities;

public class ProductAttributeEntity
{
    public long Id { get; set; }

    public long ProductId { get; set; }

    public string? AttributeGroup { get; set; }

    public string AttributeName { get; set; } = string.Empty;

    public string? AttributeValue { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
}