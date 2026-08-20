using System.Text.Json.Serialization;

namespace XTrendApp.Web.Models.Amazon;

public class AmazonColorImageJson
{
    [JsonPropertyName("large")]
    public string? Large { get; set; }

    [JsonPropertyName("thumb")]
    public string? Thumb { get; set; }

    [JsonPropertyName("hiRes")]
    public string? HiRes { get; set; }

    [JsonPropertyName("variant")]
    public string? Variant { get; set; }
}