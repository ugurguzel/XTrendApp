namespace XTrendApp.Web.Models.Product;

public class ProductListViewModel
{
    public long Id { get; set; }

    public string Name { get; set; } = "";

    public string Brand { get; set; } = "";

    public string Source { get; set; } = "";

    public string? ImageUrl { get; set; }

    public int VariationCount { get; set; }

    public decimal? Rating { get; set; }

    public int? ReviewCount { get; set; }

    public DateTime? LastCapturedAt { get; set; }
}