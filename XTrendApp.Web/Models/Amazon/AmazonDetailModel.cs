public sealed class AmazonDetailModel
{
    // Search
    public string Asin { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ProductUrl { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    // General
    public string Brand { get; set; } = string.Empty;
    public string? Collection { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;

    // Snapshot
    public decimal? Price { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal? ShippingPrice { get; set; }

    public decimal? Rating { get; set; }
    public int? ReviewCount { get; set; }

    public string? CouponText { get; set; }
    public string? DeliveryText { get; set; }

    // Detail
    public Dictionary<string, Dictionary<string, string>> Sections { get; set; }
        = new();

    // Variations
    public List<AmazonVariationItem> Variations { get; set; } = new();

    // Images
    public List<string> Images { get; set; } = new();

    // Documents
    public List<string> Documents { get; set; } = new();
}