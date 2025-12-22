using System.Text.Json.Serialization;

public class MovieCertificationResponse
{
    [JsonPropertyName("results")]
    public List<ReleaseResult> Results { get; set; }
}

public class ReleaseResult
{
    [JsonPropertyName("iso_3166_1")]
    public string Iso_3166_1 { get; set; }
    
    [JsonPropertyName("release_dates")]
    public List<ReleaseDateInfo> ReleaseDates { get; set; }
}

public class ReleaseDateInfo
{
    [JsonPropertyName("certification")]
    public string Certification { get; set; }
}