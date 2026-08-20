//namespace XTrendApp.Web.Models.Entities;

//public class BrandEntity
//{
//    public long Id { get; set; }

//    public string Name { get; set; } = string.Empty;

//    public string? Description { get; set; }

//    public string? Website { get; set; }

//    public string? LogoUrl { get; set; }

//    public bool IsActive { get; set; }

//    public DateTime CreatedAt { get; set; }

//    public DateTime? UpdatedAt { get; set; }
//}

namespace XTrendApp.Web.Models.Entities;

public class BrandEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Website { get; set; }

    public string? LogoUrl { get; set; }

    public bool IsActive { get; set; }
}