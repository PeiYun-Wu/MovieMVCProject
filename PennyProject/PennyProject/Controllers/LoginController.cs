using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using PennyProject.DataBase.MovieDB;
using PennyProject.Helper;
using PennyProject.Models;
using System.Diagnostics;


namespace PennyProject.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly PennyMovieDBContext _dbcontext;

        public LoginController(ILogger<LoginController> logger, PennyMovieDBContext dbContext)
        {
            _logger = logger;
            _dbcontext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            try
            {
                var user = await _dbcontext.UserRoles
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null)
                {
                    return Json(new simpleResponseDto
                    {
                        Success = false,
                        Message = "Sorry... Not registered yet"
                    });
                }

                if (user.Password != model.Password) 
                {
                    return Json(new simpleResponseDto
                    {
                        Success = false,
                        Message = "Password Error!"
                    });
                }

                HttpContext.Session.SetString("UserId", user.UserId);

                return Json(new simpleResponseDto
                {
                    Success = true,
                    Message = $"Login Success! .. Hi {user.UserName}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Json(new simpleResponseDto
                {
                    Success = false,
                    Message = "server error"
                });
            }
        }
    }
   
}
