using System.Text.Json.Serialization;

namespace FILMHUB.Models;

public class MovieApiResponse
{
    [JsonPropertyName("results")]
    public List<Movie> Results { get; set; }
    
    [JsonPropertyName("page")]
    public int Page { get; set; }
    
    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("total_results")]
    public int TotalResults { get; set; } 
}