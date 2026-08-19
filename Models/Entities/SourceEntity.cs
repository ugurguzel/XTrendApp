namespace XTrendApp.Web.Models.Entities;

public class SourceEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string? Website { get; set; }

    public bool IsActive { get; set; }
}