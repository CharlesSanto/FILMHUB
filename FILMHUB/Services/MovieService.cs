using FILMHUB.Models;

namespace FILMHUB.Services.Interfaces;

public class MovieService : IMovieService
{
    
    private readonly HttpClient _client;
    
    public MovieService(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<List<Movie>> GetPopularMoviesAsync()
    {
        var popularMovies = await _client.GetFromJsonAsync<MovieApiResponse>("movie/popular?language=pt-BR");
        
        return popularMovies.Results;
    }
    
    public async Task<List<Movie>> GetMoviesInTheaters()
    {
        var theatersMovies = await _client.GetFromJsonAsync<MovieApiResponse>("movie/now_playing?language=pt-BR");
        
        return theatersMovies.Results;
    }

    public async Task<List<Movie>> GetTopRatedMovies()
    {
        var topRated = await _client.GetFromJsonAsync<MovieApiResponse>("movie/top_rated?language=pt-BR");
        
        return topRated.Results;
    }

    public async Task<List<Movie>> GetTrendingMovies()
    {
        var trendingMovies = await _client.GetFromJsonAsync<MovieApiResponse>("trending/movie/week?language=pt-BR");
        
        return trendingMovies.Results;
    }
}