public class ProductVariationViewModel
{
    public long Id { get; set; }

    public string ASIN { get; set; } = string.Empty;

    public string? Name { get; set; }

    public string? Color { get; set; }

    public string? Size { get; set; }

    public decimal? Price { get; set; }

    public string? CurrencyCode { get; set; }

    public bool IsActive { get; set; }

    public string? ProductUrl { get; set; }
}