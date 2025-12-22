using System.Text.Json.Serialization;

namespace FILMHUB.Models;

public class ModeVideoResponse
{
    [JsonPropertyName("results")]
    public List<MovieVideo> Results { get; set; }
}