using System.Text.Json.Serialization;

namespace FILMHUB.Models;

public class MovieVideo
{
    [JsonPropertyName("key")]
    public string Key { get; set; }
    
    [JsonPropertyName("site")]
    public string Site { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("iso_639_1")]
    public string Iso_639_1 { get; set; }
}