using XTrendApp.Web.Models.Entities;

public class ProductImageEntity : BaseEntity
{
    public long ProductVariationId { get; set; }

    public string ImageUrl { get; set; } = "";

    public int SortOrder { get; set; }

    public bool IsPrimary { get; set; }
}