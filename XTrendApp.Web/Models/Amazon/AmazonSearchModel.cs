namespace XTrendApp.Web.Models.Amazon
{
    public class AmazonSearchModel
    {
        public string Asin { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string ProductUrl { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal? ListPrice { get; set; }
        public decimal? Rating { get; set; }
        public int? ReviewCount { get; set; }
        public string? BoughtLastMonthText { get; set; }
        public int? BoughtLastMonthCount { get; set; }
        public int? VariationCount { get; set; }
    }
}