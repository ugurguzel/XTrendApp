//namespace XTrendApp.Web.Models.Entities;

//public class ProductEntity
//{
//    public long Id { get; set; }

//    public long BrandId { get; set; }

//    public long? CollectionId { get; set; }

//    public long? CategoryId { get; set; }

//    public long SourceId { get; set; }

//    public string CountryCode { get; set; } = string.Empty;

//    public string ExternalProductId { get; set; } = string.Empty;

//    public string? SKU { get; set; }

//    public string? UPC { get; set; }

//    public string? EAN { get; set; }

//    public string? GTIN { get; set; }

//    public string Name { get; set; } = string.Empty;

//    public string? Description { get; set; }

//    public string ProductUrl { get; set; } = string.Empty;

//    public string CurrencyCode { get; set; } = string.Empty;

//    public bool IsActive { get; set; }

//    public DateTime CreatedAt { get; set; }

//    public DateTime? UpdatedAt { get; set; }

//    public DateTime? LastScanAt { get; set; }

//    public byte[]? RowVersion { get; set; }
//}

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
}