using FILMHUB.Data;
using FILMHUB.Models;
using FILMHUB.ViewModel;
using FILMHUB.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FILMHUB.Services;

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

    public async Task<MovieApiResponse?> Search(string query)
    {
        try
        {
            var movies = await _client.GetFromJsonAsync<MovieApiResponse>(
                $"search/movie?query={Uri.EscapeDataString(query)}&language=pt-BR"
            );

            if (movies == null)
                return null;

            return movies;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<MovieApiResponse> GetPopularMoviesAsync(int page = 1)
    {
        var movies = await _client.GetFromJsonAsync<MovieApiResponse>($"movie/popular?language=pt-BR&page={page}");

        if (movies == null) return null;
        
        return movies;
    }

    public Task<Movie?> GetMovieByID(int id) =>
        GetOrSetCacheAsync(
            $"movie_{id}",
            TimeSpan.FromHours(12),
            () => _client.GetFromJsonAsync<Movie>($"movie/{id}?language=pt-BR"),
            CacheItemPriority.High
        );

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

    public Task<string?> GetMovieTrailer(int movieId) =>
        GetOrSetCacheAsync(
            $"movie_{movieId}_trailer",
            TimeSpan.FromHours(12),
            async () =>
            {
                var response = await _client
                    .GetFromJsonAsync<MovieVideoResponse>($"movie/{movieId}/videos?language=pt-BR");

                return response?.Results?
                    .Where(v => v.Site == "YouTube" && v.Type == "Trailer")
                    .OrderByDescending(v => v.Official)
                    .FirstOrDefault()
                    ?.Key;
            }
        );


    public Task<string?> GetMovieCertification(int movieId) =>
        GetOrSetCacheAsync(
            $"movie_{movieId}_cert",
            TimeSpan.FromHours(24),
            async () =>
            {
                var response = await _client
                    .GetFromJsonAsync<MovieCertificationResponse>($"movie/{movieId}/release_dates");

                return response?.Results?
                    .FirstOrDefault(r => r.Iso_3166_1 == "BR")?
                    .ReleaseDates
                    .FirstOrDefault(rd => !string.IsNullOrEmpty(rd.Certification))
                    ?.Certification;
            }
        );


    public Task<List<Crew>> GetMovieCredits(int movieId) =>
        GetOrSetCacheAsync(
            $"movie_{movieId}_credits",
            TimeSpan.FromHours(24),
            async () =>
            {
                var response = await _client
                    .GetFromJsonAsync<Credits>($"movie/{movieId}/credits");

                return response?.Crew ?? new List<Crew>();
            }
        );

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

    public async Task<List<FavoriteMovieViewModel>> GetFavoriteMovies(int userId)
    {
        var ids = await _context.UserMovies
            .Where(u => u.UserId == userId && u.IsFavorite)
            .Select(u => u.MovieId)
            .ToListAsync();

        var movies = await Task.WhenAll(ids.Select(GetMovieByID));

        return movies
            .Where(m => m != null)
            .Select(m => new FavoriteMovieViewModel
            {
                MovieId = m!.Id,
                Title = m.Title,
                PosterPath = m.PosterPath,
                VoteAverage = m.VoteAverage
            })
            .ToList();
    }

    public async Task<ReviewsViewModel> GetUserReviews(int userId)
    {
        var reviews = await _context.UserMovies
            .Where(u => u.UserId == userId && u.WatchedAt != null)
            .OrderByDescending(u => u.UpdatedAt)
            .ToListAsync();

        var movies = await Task.WhenAll(
            reviews.Select(r => GetMovieByID(r.MovieId))
        );

        return new ReviewsViewModel
        {
            Movies = movies.Where(m => m != null).ToList(),
            UserMovies = reviews
        };
    }

    public async Task<List<Movie?>> GetUserWatchList(int userId)
    {
        var movieIds = await _context.UserMovies
            .Where(um => um.UserId == userId && um.Status == UserMovieStatus.WantToWatch)
            .OrderByDescending(um => um.UpdatedAt)
            .Select(um => um.MovieId)
            .Distinct()
            .ToListAsync();

        var movieTasks = movieIds.Select(id => GetMovieByID(id));

        return (await Task.WhenAll(movieTasks))
            .Where(m => m != null)
            .ToList();
    }
    
    private async Task<T?> GetOrSetCacheAsync<T>(
        string key,
        TimeSpan ttl,
        Func<Task<T?>> factory,
        CacheItemPriority priority = CacheItemPriority.Normal
    )
    {
        if (_cache.TryGetValue(key, out T cached))
            return cached;

        var result = await factory();

        _cache.Set(key, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl,
            Priority = priority
        });

        return result;
    }

}