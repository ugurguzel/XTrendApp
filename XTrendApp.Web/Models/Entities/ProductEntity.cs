namespace XTrendApp.Web.Models.Entities;

public class ProductEntity : BaseEntity
{
    public long SourceId { get; set; }

    public long BrandId { get; set; }

    public long? CollectionId { get; set; }

    public long? CategoryId { get; set; }

    public string SourceProductId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    // SQL Server rowversion (timestamp)
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public string? ProductUrl { get; set; }
}