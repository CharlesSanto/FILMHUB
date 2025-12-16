using FILMHUB.Models;

namespace FILMHUB.Services.Interfaces;

public class MovieService : IMovieService
{
    
    private readonly HttpClient _client;
    
    public MovieService(HttpClient client)
    {
        _client = client;
    }

    private async Task<List<Movie>> GetMovies(string endpoint)
    {
        var movieResponse = await _client.GetFromJsonAsync<MovieApiResponse>($"{endpoint}?language=pt-BR");
        return movieResponse?.Results ?? new List<Movie>();
    }
    
    public async Task<List<Movie>> GetPopularMoviesAsync() => await GetMovies("movie/popular");

    public async Task<List<Movie>> GetMoviesInTheaters() => await GetMovies("movie/now_playing");

    public async Task<List<Movie>> GetTopRatedMovies() => await GetMovies("movie/top_rated");

    public async Task<List<Movie>> GetTrendingMovies() => await GetMovies("trending/movie/week");
}