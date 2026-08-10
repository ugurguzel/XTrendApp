namespace XTrendApp.Web.Models.Product;

public class ProductDetailViewModel
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Brand { get; set; }

    public string? Source { get; set; }

    public string? ProductUrl { get; set; }

    public decimal? Rating { get; set; }

    public int? ReviewCount { get; set; }

    public int VariationCount { get; set; }

    public DateTime? LastCapturedAt { get; set; }

    public List<ProductAttributeViewModel> Attributes { get; set; } = new();

    public List<ProductVariationViewModel> Variations { get; set; } = new();

    public List<ProductReviewHistoryViewModel> ReviewHistory { get; set; } = new();
}