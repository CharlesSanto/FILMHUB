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
        // https://api.themoviedb.org/3/movie/popular
        
        var movieResponse = await _client.GetFromJsonAsync<MovieApiResponse>("movie/popular");
        
        return movieResponse.Results;
    }
}