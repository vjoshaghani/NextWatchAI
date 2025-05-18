using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextWatchAI.Data;
using NextWatchAI.Data.Entities;
using NextWatchAI.Models;

namespace NextWatchAI.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FavoriteController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Index", "Home");

            var favorites = await _context.Favorites
                .Where(f => f.UserId == user.Id)
                .Include(f => f.Movie)
                .ToListAsync();

            var vmList = favorites.Select(f => new FavoriteMovieVM
            {
                FavoriteId = f.FavoriteId,
                Id = f.Movie.TmdbId,
                Title = f.Movie.Title,
                PosterUrl = f.Movie.PosterUrl,
                Overview = f.Movie.Overview,
                Note = f.Note
            }).ToList();

            return View(vmList);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int tmdbId, string? returnUrl)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Index", "Home");

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.TmdbId == tmdbId);
            if (movie != null)
            {
                var fav = await _context.Favorites
                    .FirstOrDefaultAsync(f
                        => f.UserId == user.Id
                        && f.MovieId == movie.MovieId);
                if (fav != null)
                {
                    _context.Favorites.Remove(fav);
                    await _context.SaveChangesAsync();
                }
            }

            return Redirect(returnUrl ?? Url.Action("Index") ?? "/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNote(int id, string note)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.FavoriteId == id && f.UserId == user.Id);
            if (favorite == null)
            {
                return RedirectToAction("Index");
            }

            favorite.Note = note;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
