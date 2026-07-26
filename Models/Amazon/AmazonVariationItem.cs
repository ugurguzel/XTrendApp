public sealed class AmazonVariationItem
{
    public string Value { get; set; } = "";
    public string? Asin { get; set; }
    public bool Selected { get; set; }
    public bool Available { get; set; }

    public decimal? Price { get; set; }
    public decimal? ListPrice { get; set; }
    public string? Currency { get; set; }

    public string? ImageUrl { get; set; }
}