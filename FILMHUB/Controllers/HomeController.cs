using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FILMHUB.Models;
using FILMHUB.Services.Interfaces;
using FILMHUB.ViewModel;

namespace FILMHUB.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMovieService _movieService;

    public HomeController(ILogger<HomeController> logger, IMovieService movieService)
    {
        _logger = logger;
        _movieService = movieService;
    }

    public async Task<IActionResult> Index()
    {
        HomeViewModel movieResponse = new HomeViewModel()
        {
            MoviesInTheaters = await _movieService.GetMoviesInTheaters(),
            TopRatedMovies = await _movieService.GetTopRatedMovies(),
            TrendingMovies = await _movieService.GetTrendingMovies(),
            BannerMovie = await _movieService.GetRandomBannerMovie()
        };
        
        return View(movieResponse);
    }

    public async Task<IActionResult> MovieDetails(int id)
    {
        Movie movie = await _movieService.GetMovieByID(id);
        var trailer = await _movieService.GetMovieTrailer(id);
        var certification =  await _movieService.GetMovieCertification(id);
        
        if (movie == null) return NotFound();

        UserMovie? userMovie = null;
        
        int? userId = HttpContext.Session.GetInt32("UserId");

        if (userId.HasValue)
        {
            userMovie = await _movieService.GetUserAndMovie(userId, movie.Id);
        }

        return View(new MovieDetailsViewModels
        {
            Movie = movie,
            UserMovie = userMovie,
            Trailer = trailer,
            Certification = certification
        });
    }

    public async Task<IActionResult> Movies(int page = 1)
    {
        MovieApiResponse movies = null;
        int currentPage = page;
        int maxPage = 10;

        while (currentPage <= maxPage)
        {
            try
            {
                movies = await _movieService.GetPopularMoviesAsync(currentPage);
                if (movies?.Results != null && movies.Results.Any())
                    break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na página {currentPage}: {ex.Message}");
            }
            currentPage++;
        }

        if (movies == null || !movies.Results.Any())
        {
            movies = new MovieApiResponse { Page = 1, Results = new List<Movie>() };
        }

        return View(movies);
    }

    public IActionResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}