namespace XTrendApp.Web.Connectors.Amazon
{
    public class AmazonOptions
    {
        public string BaseUrl { get; set; } = string.Empty;

        public string Keyword { get; set; } = "Rugs";

        public int MaxPage { get; set; } = 1;

        //public int MaxProducts { get; set; } = 50;

        public bool Headless { get; set; }

        public string SessionFile { get; set; } = "amazon-us.json";

        public string SortOrder { get; set; } = "exact-aware-popularity-rank";


    }
}