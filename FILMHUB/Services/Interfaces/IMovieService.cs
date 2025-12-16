using FILMHUB.Models;

namespace FILMHUB.Services.Interfaces;

public interface IMovieService
{
    Task<List<Movie>> GetPopularMoviesAsync();
}