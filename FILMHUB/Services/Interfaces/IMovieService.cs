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
    Task<string?> GetMovieTrailer(int movieId);
    Task<string> GetMovieCertification(int movieId);
    Task<List<Crew>> GetMovieCredits(int movieId);
    Task SetStatus (int userId, int movieId, UserMovieStatus status);
    Task IsFavorite(int userId, int movieId, bool status);
}