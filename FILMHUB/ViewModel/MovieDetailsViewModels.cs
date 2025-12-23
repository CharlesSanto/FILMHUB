using FILMHUB.Models;

namespace FILMHUB.ViewModel;

public class MovieDetailsViewModels
{
    public List<UserMovie> RecentReviews { get; set; }
    public Movie Movie { get; set; }
    public UserMovie UserMovie { get; set; }
    public string Trailer { get; set; }
    public string Certification { get; set; }
    public List<Crew> Crew { get; set; }
    
    public bool HasUserMovie => UserMovie != null;
}