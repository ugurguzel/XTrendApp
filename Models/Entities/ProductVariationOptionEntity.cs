namespace XTrendApp.Web.Models.Entities;

public class ProductVariationOptionEntity : BaseEntity
{
    public long ProductVariationId { get; set; }

    public string OptionName { get; set; } = string.Empty;

    public string OptionValue { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
}