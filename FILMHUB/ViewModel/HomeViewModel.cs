using FILMHUB.Models;

namespace FILMHUB.ViewModel;

public class HomeViewModel
{
    public List<Movie> PopularMovies { get; set; }
    public List<Movie> MoviesInTheaters { get; set; }
    public List<Movie> TopRatedMovies { get; set; }
    public List<Movie> TrendingMovies { get; set; }
}