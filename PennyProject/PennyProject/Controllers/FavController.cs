using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PennyProject.DataBase.MovieDB;
using PennyProject.Models;
using PennyProject.Repo;

namespace PennyProject.Controllers
{
   
    public class FavController : Controller
    {
        private readonly PennyMovieDBContext _dbContext;
        private readonly ILogger<FavController> _logger;
        private readonly IFavService _favService;

        public FavController(PennyMovieDBContext dbContext, ILogger<FavController> logger, IFavService favService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _favService = favService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Index", "Auth");
                }
                var favoriteMovies = await _favService.GetUserFavoritesAsync(userId);

                return View(favoriteMovies);
            }
            catch (Exception ex) {

                _logger.LogError($"favorite page errer : {ex}");

                return Json(new ApiResponseDto
                {
                    Success = false,
                    Message = "Get data failed!  go check error log!  "
                });
            }
        }

        /// <summary>
        /// 新增最愛電影
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        [HttpPost("api/Favorite")]
        public async Task<IActionResult> AddFavorite(string movieId)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Index", "Auth");
                }
                var movieInfo = await _favService.AddFavoriteAsync(userId, movieId);

               return Json(movieInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"MovieId {movieId}, add favorite failed: {ex}");
                return Json(new ApiResponseDto
                {
                    Success = false,
                    Message = "oh no.. add favorite movie failed! go check error log! "
                });
            }
        }

        /// <summary>
        /// 移除最愛電影
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        [HttpDelete("api/Favorite")]
        public async Task<IActionResult> RemoveFavorite(string movieId)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Index", "Auth");
                }
                var deleteInfo = await _favService.RemoveFavoriteAsync(userId, movieId);
                return Json(deleteInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"MovieId: {movieId} Remove favorite failed: {ex}");
                return Json(new ApiResponseDto
                {
                    Success = false,
                    Message = "delete failed!  go check error log!  "
                });
            }
        }
    }
}
