using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextWatchAI.Data;
using NextWatchAI.Data.Entities;
using NextWatchAI.Models;
using NextWatchAI.Services;

namespace NextWatchAI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly TmdbMovieService _movieService;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(ILogger<HomeController> logger, TmdbMovieService movieService, ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _movieService = movieService;
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _movieService.GetPopularMoviesAsync(page);
        ViewData["CurrentPage"] = page;
        ViewData["TotalPages"] = result.TotalPages;

        HashSet<int> favoriteIds = new();
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                favoriteIds = _context.Favorites
                    .Where(f => f.UserId == user.Id)
                    .Select(f => f.Movie.TmdbId)  
                    .ToHashSet();
            }
        }

        ViewData["FavoriteIds"] = favoriteIds;
        return View(result.Movies);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> Search(string query, int page = 1, string? aiReasoning = null)
    {
        if (string.IsNullOrWhiteSpace(query))
            return RedirectToAction("Index");

        var result = await _movieService.SearchMoviesAsync(query, page);

        ViewData["SearchQuery"] = query;
        ViewData["CurrentPage"] = page;
        ViewData["TotalPages"] = result.TotalPages;

        if (!string.IsNullOrWhiteSpace(aiReasoning))
            ViewData["AIReasoning"] = aiReasoning;

        return View("Index", result.Movies);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToFavorites(int movieId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["Error"] = "Please log in to add favorites.";
            return RedirectToAction("Index");
        }

        var movie = await _context.Movies.FirstOrDefaultAsync(m => m.TmdbId == movieId);
        if (movie == null)
        {
            var tmdbMovie = await _movieService.GetMovieByIdAsync(movieId);
            if (tmdbMovie == null)
            {
                TempData["Error"] = "Movie not found.";
                return RedirectToAction("Index");
            }

            movie = new Movie
            {
                TmdbId = tmdbMovie.Id,
                Title = tmdbMovie.Title,
                PosterUrl = tmdbMovie.PosterUrl,
                Overview = tmdbMovie.Overview
            };
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
        }

        var exists = await _context.Favorites.AnyAsync(f => f.UserId == user.Id && f.MovieId == movie.MovieId);
        if (!exists)
        {
            var favorite = new Favorite { UserId = user.Id, MovieId = movie.MovieId };
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
}
