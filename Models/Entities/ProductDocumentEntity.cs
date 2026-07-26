namespace XTrendApp.Web.Models.Entities;

public class ProductDocumentEntity
{
    public long Id { get; set; }

    public long ProductId { get; set; }

    public string DocumentType { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string DocumentUrl { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}