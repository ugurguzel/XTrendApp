//namespace XTrendApp.Web.Models.Entities;

//public class CollectionEntity
//{
//    public long Id { get; set; }

//    public long BrandId { get; set; }

//    public string Name { get; set; } = string.Empty;

//    public string? Description { get; set; }

//    public bool IsActive { get; set; }

//    public DateTime CreatedAt { get; set; }

//    public DateTime? UpdatedAt { get; set; }
//}

namespace XTrendApp.Web.Models.Entities;

public class CollectionEntity : BaseEntity
{
    public long BrandId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }
}