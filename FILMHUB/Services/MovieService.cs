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
        var movies = movieResponse?.Results.Take(10).ToList() ?? new List<Movie>();

        _cache.Set(cacheKey, movies, TimeSpan.FromMinutes(10));

        return movies;
    }

    public async Task<List<Movie>> GetMoviesInTheaters() => await GetMovies("movie/now_playing");

    public async Task<List<Movie>> GetTopRatedMovies() => await GetMovies("movie/top_rated");

    public async Task<List<Movie>> GetTrendingMovies() => await GetMovies("trending/movie/week");
    
    public async Task<MovieApiResponse> GetPopularMoviesAsync(int page = 1)
    {
        var movies = await _client.GetFromJsonAsync<MovieApiResponse>($"movie/popular?language=pt-BR&page={page}");

        if (movies == null) return null;
        
        return movies;
    }

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
            .FirstOrDefaultAsync(um =>
                um.UserId == userId &&
                um.MovieId == movieId
            );
    }

    public async Task<string> GetMovieTrailer(int movieId)
    {
        var videoResponse = await _client.GetFromJsonAsync<MovieVideoResponse>($"movie/{movieId}/videos?language=pt-BR");

        if (videoResponse?.Results == null || !videoResponse.Results.Any()) 
            return null;

        var trailerFinal = videoResponse.Results
            .Where(v => v.Site == "YouTube" && v.Type == "Trailer")
            .OrderByDescending(v => v.Name.Contains("Dublado", StringComparison.OrdinalIgnoreCase)) 
            .ThenByDescending(v => v.Official)
            .ThenByDescending(v => v.Iso_639_1 == "pt")
            .FirstOrDefault();

        if (trailerFinal == null)
        {
            var fallbackResponse = await _client.GetFromJsonAsync<MovieVideoResponse>($"movie/{movieId}/videos");
            trailerFinal = fallbackResponse?.Results
                .FirstOrDefault(v => v.Site == "YouTube" && v.Type == "Trailer");
        }

        return trailerFinal?.Key;
    }

    public async Task<string> GetMovieCertification(int movieId)
    {
        var response = await _client.GetFromJsonAsync<MovieCertificationResponse>(
            $"movie/{movieId}/release_dates"
        );
        
        var br = response?.Results
            ?.FirstOrDefault(r => r.Iso_3166_1 == "BR");

        var certification = br?.ReleaseDates
            .FirstOrDefault(rd => !string.IsNullOrEmpty(rd.Certification))
            ?.Certification;
        return certification;
    }

    public async Task<List<Crew>> GetMovieCredits(int movieId)
    {
        var creadits = await _client.GetFromJsonAsync<Credits>($"movie/{movieId}/credits");
        
        return creadits?.Crew ?? new List<Crew>();
        
    }

    public async Task SetStatus(int userId, int movieId, UserMovieStatus status)
    {
        var userMovie = await _context.UserMovies.FirstOrDefaultAsync(um => um.UserId == userId && um.MovieId == movieId);

        if (userMovie == null)
        {
            userMovie = new UserMovie()
            {
                UserId = userId,
                MovieId = movieId,
                Status = status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.UserMovies.Add(userMovie);
        }
        else
        {
            userMovie.Status = status;
            userMovie.UpdatedAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
    }

    public async Task IsFavorite(int userId, int movieId, bool status)
    {
        var userMovie = await  _context.UserMovies.FirstOrDefaultAsync(um => um.UserId == userId && um.MovieId == movieId);

        if (userMovie == null)
        {
            userMovie = new UserMovie()
            {
                UserId = userId,
                MovieId = movieId,
                Status = UserMovieStatus.None,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsFavorite = status,
            };
            
            _context.UserMovies.Add(userMovie);
        }
        else
        {
            userMovie.IsFavorite = status;
            userMovie.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task SaveReview(int userId, int movieId, int rating, DateTime watchedAt, string comment)
    {
        var userMovie = await _context.UserMovies.FirstOrDefaultAsync(um => um.UserId == userId && um.MovieId == movieId);

        if (userMovie == null)
        {
            userMovie = new UserMovie()
            {
                UserId = userId,
                MovieId = movieId,
                Rating = rating,
                WatchedAt = watchedAt,
                Review =  comment,
                Status = UserMovieStatus.Watched,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            
            await _context.UserMovies.AddAsync(userMovie);
        }
        else
        {
            userMovie.UserId = userId;
            userMovie.MovieId = movieId;
            userMovie.Rating = rating;
            userMovie.WatchedAt = watchedAt;
            userMovie.Review = comment;
            userMovie.Status = UserMovieStatus.Watched;
            userMovie.UpdatedAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserMovie>> GetRecentReviwes(int movieId)
    {
        var userMovie = await _context.UserMovies
            .Include(um => um.User)
            .Where(um => !string.IsNullOrEmpty(um.Review) && um.MovieId == movieId)
            .OrderByDescending(um => um.UpdatedAt)
            .Take(10)
            .ToListAsync();
        
        return  userMovie;
    }
}