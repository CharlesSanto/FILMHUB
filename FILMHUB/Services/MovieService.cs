using FILMHUB.Models;
using Microsoft.Extensions.Caching.Memory;

namespace FILMHUB.Services.Interfaces;

public class MovieService : IMovieService
{
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;
    
    public MovieService(HttpClient client, IMemoryCache cache)
    {
        _client = client;
        _cache = cache;
    }

    private async Task<List<Movie>> GetMovies(string endpoint)
    {
        var cacheKey = $"movies_{endpoint}";

        if (_cache.TryGetValue(cacheKey, out List<Movie> cachedmovies))
        {
            return cachedmovies;
        }
        
        var movieResponse = await _client.GetFromJsonAsync<MovieApiResponse>($"{endpoint}?language=pt-BR");
        var movies = movieResponse?.Results ?? new List<Movie>();

        _cache.Set(cacheKey, movies, TimeSpan.FromMinutes(10));

        return movies;
    }
    
    public async Task<List<Movie>> GetPopularMoviesAsync() => await GetMovies("movie/popular");

    public async Task<List<Movie>> GetMoviesInTheaters() => await GetMovies("movie/now_playing");

    public async Task<List<Movie>> GetTopRatedMovies() => await GetMovies("movie/top_rated");

    public async Task<List<Movie>> GetTrendingMovies() => await GetMovies("trending/movie/week");
}