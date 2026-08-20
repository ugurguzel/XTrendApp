namespace XTrendApp.Web.Models.Amazon
{
    public sealed class AmazonVariationGroup
    {
        public string Name { get; set; } = string.Empty;

        public string SelectedValue { get; set; } = string.Empty;

        public List<AmazonVariationItem> Items { get; set; } = new();
    }
}