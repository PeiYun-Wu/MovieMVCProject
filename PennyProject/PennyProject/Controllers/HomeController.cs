using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PennyProject.DataBase.MovieDB;
using PennyProject.Models;
using System.Diagnostics;
using PennyProject.Models.Enum;
using static PennyProject.Models.Enum.CountryOrderEnum;

namespace PennyProject.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PennyMovieDBContext _dbcontext;
        public HomeController(ILogger<HomeController> logger, PennyMovieDBContext dbContext)
        {
            _logger = logger;
            _dbcontext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Login");
                }

                var userFavorites = await _dbcontext.UserFavorites
                                .Where(f => f.MemberId == userId)
                                .Select(f => f.MovieId)
                                .ToListAsync();
                ViewBag.UserFavorites = userFavorites;

                var movies = await _dbcontext.MovieInfos.ToListAsync();

                var moviesByCountry = movies
                    .GroupBy(m => m.Country)
                    .OrderBy(g => GetCountryOrder(g.Key))
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderByDescending(m => m.ReleaseDateTime).ToList()
                    );


                return View(moviesByCountry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), ex.StackTrace);
            }
            return View();
           
        }
        private int GetCountryOrder(string country)
        {
            return country?.ToLower() switch
            {
                "chiness" => (int)CountryOrder.Chiness,
                "americas" => (int)CountryOrder.Americas,
                "japan" => (int)CountryOrder.Japan,
                "korea" => (int)CountryOrder.Korea,
                "europe" => (int)CountryOrder.Europe,
                _ => 999
            };
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
