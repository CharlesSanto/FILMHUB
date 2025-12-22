using System.Text.Json.Serialization;

namespace FILMHUB.Models;

public class MovieVideo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("key")]
    public string Key { get; set; }
    
    [JsonPropertyName("site")]
    public string Site { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("iso_639_1")]
    public string Iso_639_1 { get; set; }
    
    [JsonPropertyName("iso_3166_1")]
    public string Iso_3166_1 { get; set; }

    [JsonPropertyName("official")]
    public bool Official { get; set; } 
}