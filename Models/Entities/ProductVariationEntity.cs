//namespace XTrendApp.Web.Models.Entities;

//public class ProductVariationEntity
//{
//    public long Id { get; set; }

//    public long ProductId { get; set; }

//    public string ExternalVariationId { get; set; } = string.Empty;

//    public string? SKU { get; set; }

//    public string? VariantName { get; set; }

//    public string? Color { get; set; }

//    public string? Size { get; set; }

//    public string? Style { get; set; }

//    public string? VariationUrl { get; set; }

//    public decimal? Price { get; set; }

//    public decimal? ListPrice { get; set; }

//    public string? CurrencyCode { get; set; }

//    public string? DeliveryText { get; set; }

//    public bool IsDefault { get; set; }

//    public bool IsActive { get; set; }

//    public DateTime CreatedAt { get; set; }

//    public DateTime? UpdatedAt { get; set; }
//}

using XTrendApp.Web.Models.Entities;

public class ProductVariationEntity : BaseEntity
{
    public long ProductId { get; set; }

    public string SourceVariationId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? SKU { get; set; }

    public string? UPC { get; set; }

    public string? EAN { get; set; }

    public string? GTIN { get; set; }

    public string? ProductUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}