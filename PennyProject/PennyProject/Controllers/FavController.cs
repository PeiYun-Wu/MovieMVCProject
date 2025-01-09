using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PennyProject.DataBase.MovieDB;
using PennyProject.Models;

namespace PennyProject.Controllers
{
   
    public class FavController : Controller
    {
        private readonly PennyMovieDBContext _dbContext;
        private readonly ILogger<FavController> _logger;

        public FavController(PennyMovieDBContext dbContext, ILogger<FavController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");  
            }

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Login");
            }

            var favoriteMovies = await _dbContext.MovieInfos
                .Join(_dbContext.UserFavorites.Where(f => f.MemberId == userId),
                      m => m.MovieId,
                      f => f.MovieId,
                      (m, f) => m)
                .OrderByDescending(m => m.ReleaseDateTime)
                .ToListAsync();

            return View(favoriteMovies);
        }

        [HttpPost("api/Favorite")]
        public async Task<IActionResult> AddFavorite(string movieId)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                
                var movie = await _dbContext.MovieInfos.FirstOrDefaultAsync(m => m.MovieId == movieId);
                var newFavorite = new UserFavorite
                {
                    MemberId = userId,
                    MovieId = movieId,
                    MovieChinessName = movie.MovieChinessName,
                    CreateDateTime = DateTime.Now
                };

                _dbContext.UserFavorites.Add(newFavorite);
                await _dbContext.SaveChangesAsync();

                _logger.LogDebug($"Added favorite movie: {movieId} for user: {userId}");
                return Json(new simpleResponseDto
                {
                    Success = true,
                    Message = "add success! "
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"MovieId {movieId}, add favorite failed: {ex}");
                return Json(new simpleResponseDto
                {
                    Success = false,
                    Message = "oh no.. add favorite movie failed! go check error log! "
                });
            }
        }
        [HttpDelete("api/Favorite")]
        public async Task<IActionResult> RemoveFavorite(string movieId)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
             

                var favorite = await _dbContext.UserFavorites
                    .FirstOrDefaultAsync(f => f.MemberId == userId && f.MovieId == movieId);

                if (favorite != null)
                {
                    _dbContext.UserFavorites.Remove(favorite);
                    await _dbContext.SaveChangesAsync();

                    _logger.LogDebug($"MovieId {movieId}, Removed favorite movie: {movieId} for user: {userId}");
                    return Json(new simpleResponseDto
                    {
                        Success = true,
                        Message = "remove success! "
                    });
                }

                return Json(new simpleResponseDto
                {
                    Success = false,
                    Message = "can'f find this record! "
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"MovieId: {movieId} Remove favorite failed: {ex}");
                return Json(new simpleResponseDto
                {
                    Success = false,
                    Message = "delete failed!  go check error log!  "
                });
            }
        }
    }
}
