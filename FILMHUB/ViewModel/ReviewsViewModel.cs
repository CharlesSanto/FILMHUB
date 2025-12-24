using FILMHUB.Models;

namespace FILMHUB.ViewModel;

public class ReviewsViewModel
{
    public List<Movie> Movies { get; set; }
    public List<UserMovie> UserMovies { get; set; }
}