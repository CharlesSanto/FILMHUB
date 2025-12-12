using System.ComponentModel.DataAnnotations;

namespace FILMHUB.Models;

public class UserMovie
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User? User { get; set; }
    
    public int MovieId { get; set; }
    
    public double? Rating { get; set; } 
    [MaxLength(2000)]
    public string? Review { get; set; }
    public UserMovieStatus Status { get; set; }
    public DateTime? WatchedAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}