namespace XTrendApp.Web.Models.Amazon
{
    public sealed class AmazonVariationResult
    {
        public string ParentAsin { get; set; } = string.Empty;

        public List<AmazonVariationSize> Sizes { get; set; } = new();

        public HashSet<string> ChildAsins { get; set; } = new();
    }
}