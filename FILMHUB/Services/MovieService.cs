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
        var popularMovies = await _client.GetFromJsonAsync<MovieApiResponse>("movie/popular");
        
        return popularMovies.Results;
    }
    
    public async Task<List<Movie>> GetMoviesInTheaters()
    {
        var theatersMovies = await _client.GetFromJsonAsync<MovieApiResponse>("movie/now_playing");
        
        return theatersMovies.Results;
    }
}