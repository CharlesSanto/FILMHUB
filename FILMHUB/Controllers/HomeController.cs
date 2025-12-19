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
            UserMovie = userMovie
        });
    }

    public async Task<IActionResult> Movies()
    {
        var movies = await _movieService.GetPopularMoviesAsync();
        
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