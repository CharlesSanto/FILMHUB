using System.Text.Json.Serialization;

namespace FILMHUB.Models;

public class Credits
{
    public List<Crew> Crew { get; set; }
}

public class Crew
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("job")]
    public string Job { get; set; }
    
    [JsonPropertyName("profile_path")]
    public string ProfilePath { get; set; }
}