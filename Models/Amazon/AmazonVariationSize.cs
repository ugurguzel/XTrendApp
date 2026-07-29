using XTrendApp.Web.Models.Amazon;

public class AmazonVariationSize
{
    public string Name { get; set; } = "";

    //public string? Title { get; set; }

    public string Asin { get; set; } = "";

    public bool Selected { get; set; }

    public bool Available { get; set; }

    // Dropdown parser için
    public int OptionIndex { get; set; }

    public string OptionValue { get; set; } = "";

    public string Href { get; set; } = "";

    public List<AmazonVariationColor> Colors { get; set; } = new();

    // Sprint-14
    public decimal? CurrentPrice { get; set; }

    public string CurrencyCode { get; set; } = "";

    public bool InStock { get; set; }

    public decimal? Price { get; set; }

    public decimal? ListPrice { get; set; }

    public string? DeliveryText { get; set; }

    public string? ImageUrl { get; set; }

}