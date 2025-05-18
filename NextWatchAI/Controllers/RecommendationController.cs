using Microsoft.AspNetCore.Mvc;
using NextWatchAI.Services;

public class RecommendationController : Controller
{
    private readonly IAiRecommendationService _aiService;
    private readonly TmdbMovieService _movieService;

    public RecommendationController(IAiRecommendationService aiService, TmdbMovieService movieService)
    {
        _aiService = aiService;
        _movieService = movieService;
    }

    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Ask(string mood)
    {
        var aiResult = await _aiService.RecommendMovieAsync(mood);

        var searchResult = await _movieService.SearchMoviesAsync(aiResult.MovieTitle, 1);
        var movie = searchResult.Movies.FirstOrDefault();

        ViewData["AiResult"] = aiResult;
        ViewData["Movie"] = movie;
        return View("Index");
    }
}
