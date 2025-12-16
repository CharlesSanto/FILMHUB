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
            PopularMovies = await _movieService.GetPopularMoviesAsync(),
            MoviesInTheaters = await _movieService.GetMoviesInTheaters()
        };
        
        return View(movieResponse);
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