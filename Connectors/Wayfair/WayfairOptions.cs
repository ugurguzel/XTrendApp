namespace XTrendApp.Web.Connectors.Wayfair
{
    public class WayfairOptions
    {
        public string BaseUrl { get; set; } = "https://www.wayfair.com";

        public string Keyword { get; set; } = "rugs";

        public bool Headless { get; set; }

        public string SessionFile { get; set; } = "wayfair.json";
    }
}