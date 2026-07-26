namespace XTrendApp.Web.Connectors.Amazon
{
    public class AmazonProduct
    {
        public string Asin { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal? Rating { get; set; }

        public int ReviewCount { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string ProductUrl { get; set; } = string.Empty;
    }
}