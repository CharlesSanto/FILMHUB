using System.Text.Json.Serialization;

namespace FILMHUB.Models;

public class Movie
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("genres")]
    public List<Genre> Genres { get; set; }
    
    [JsonPropertyName("overview")]
    public string Overview { get; set; }
    
    [JsonPropertyName("release_date")]
    public DateTime? ReleaseDate { get; set; }
    
    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; }
    
    [JsonPropertyName("backdrop_path")]
    public string BackdropPath { get; set; }
    
    [JsonPropertyName("runtime")]
    public int Runtime { get; set; }
    
    [JsonPropertyName("vote_average")]
    public int VoteAverage { get; set; }
}