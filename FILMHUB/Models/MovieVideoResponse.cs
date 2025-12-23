using System.Text.Json.Serialization;

namespace FILMHUB.Models;

public class MovieVideoResponse
{
    [JsonPropertyName("results")]
    public List<MovieVideo> Results { get; set; }
}