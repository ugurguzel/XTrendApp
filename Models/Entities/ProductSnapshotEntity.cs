namespace XTrendApp.Web.Models.Entities;

public class ProductSnapshotEntity : BaseEntity
{
    public long ProductVariationId { get; set; }

    public decimal? Price { get; set; }

    public decimal? ListPrice { get; set; }

    public decimal? SalePrice { get; set; }

    public decimal? ShippingPrice { get; set; }

    public string? CurrencyCode { get; set; }

    public decimal? Rating { get; set; }

    public int? ReviewCount { get; set; }

    public int? StockQuantity { get; set; }

    public bool? IsInStock { get; set; }

    public bool? IsPrime { get; set; }

    public bool? HasBuyBox { get; set; }

    public string? SellerName { get; set; }

    public string? BoughtLastMonthText { get; set; }

    public int? BoughtLastMonthCount { get; set; }

    public string? CouponText { get; set; }

    public string? DeliveryText { get; set; }

    public DateTime CapturedAt { get; set; }

    public long? ScanJobId { get; set; }
}