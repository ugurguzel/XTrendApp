using XTrendApp.Web.Models.Entities;

public class ProductImageEntity : BaseEntity
{
    public long ProductId { get; set; }

    public string ImageUrl { get; set; } = "";

    public string ImageType { get; set; } = "Main";

    public int DisplayOrder { get; set; }

    public bool IsPrimary { get; set; }
}