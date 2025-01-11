using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PennyProject.DataBase.MovieDB;
using PennyProject.Models;
using System.Diagnostics;
using PennyProject.Models.Enum;
using static PennyProject.Models.Enum.CountryOrderEnum;
using PennyProject.Repo;
using static PennyProject.Models.MoviePageDto;

namespace PennyProject.Controllers
{
   
    public class MovieController : Controller
    {
        private readonly ILogger<MovieController> _logger;
        private readonly PennyMovieDBContext _dbcontext;
        private readonly IMovieService _movieService;
        public MovieController(ILogger<MovieController> logger, PennyMovieDBContext dbContext, IMovieService movieService)
        {
            _logger = logger;
            _dbcontext = dbContext;
            _movieService = movieService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                var movieInfo = await _movieService.GetHomePageDataAsync(userId);

                return View(movieInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), ex.StackTrace);
                return View(new HomePageDto
                {
                    MoviesByCountry = new Dictionary<string, List<MovieDto>>(),
                    UserFavorites = new List<string>()
                });
            }
           
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
    }
}
