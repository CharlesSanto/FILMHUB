using FILMHUB.Models;

namespace FILMHUB.Services.Interfaces;

public interface IMovieService
{
    Task<MovieApiResponse> GetPopularMoviesAsync(int page);
    Task<List<Movie>> GetMoviesInTheaters();
    Task<List<Movie>> GetTopRatedMovies();
    Task<List<Movie>> GetTrendingMovies();
    Task<Movie> GetRandomBannerMovie();
    Task<Movie> GetMovieByID(int id);
    Task<UserMovie> GetUserAndMovie(int? userId, int movieId);
}