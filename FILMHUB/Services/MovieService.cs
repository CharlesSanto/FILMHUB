using FILMHUB.Data;
using FILMHUB.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FILMHUB.Services.Interfaces;

public class MovieService : IMovieService
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;
    
    public MovieService(HttpClient client, IMemoryCache cache, ApplicationDbContext context)
    {
        _client = client;
        _cache = cache;
        _context = context;
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

    public async Task<Movie> GetMovieByID(int id)
    {
        var movie = await _client.GetFromJsonAsync<Movie>($"movie/{id}?language=pt-BR");
        
        return movie;
    }

    public async Task<Movie> GetRandomBannerMovie()
    {
        const string cacheKey = "banner_movie";

        if (_cache.TryGetValue(cacheKey, out Movie cachedmovies))
        {
            return cachedmovies;
        }
        
        var movies = await GetMovies("movie/popular");

        if (movies == null || movies.Count == 0)
            return null;

        var random = new Random();
        var bannerMovie = movies[random.Next(movies.Count)];
        
        _cache.Set(cacheKey, bannerMovie, TimeSpan.FromMinutes(30));

        return bannerMovie;
    }

    public async Task<UserMovie> GetUserAndMovie(int? userId, int movieId)
    {
        return await _context.UserMovies
            .FirstOrDefaultAsync(user => user.Id == userId && movieId == user.MovieId);
    }
}