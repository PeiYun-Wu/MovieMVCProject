using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PennyProject.DataBase.MovieDB;
using PennyProject.Models;


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

        public async Task<IActionResult> UserIndex()
        {
            var userId = HttpContext.Session.GetString("UserId");
            
            var userInfo = await _dbcontext.UserRoles.Where(u=>u.UserId == userId).FirstOrDefaultAsync();

            return View(userInfo);
        }

        [HttpPost("api/Login")]
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
                _logger.LogDebug($"login user : {user.UserName}");

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

        [HttpPost("api/UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateRequestDto model)
        {
            try
            {
                //var userId = HttpContext.Session.GetString("UserId");

                var existingUser = await _dbcontext.UserRoles
                                   .Where(u => u.UserId == model.UserId && u.UserName == model.UserName)
                                   .FirstOrDefaultAsync();

                if (existingUser == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                existingUser.UserName = model.UserName;
                existingUser.Password = model.Password;  
                existingUser.Email = model.Email;
                existingUser.Age = model.Age;
                existingUser.UpdateDateTime = DateTime.Now;

                await _dbcontext.SaveChangesAsync();
                _logger.LogDebug($"UserName:{model.UserName}, UserInfo Update Susscessful!");

                return Ok(new simpleResponseDto
                {
                    Success = true,
                    Message = "UserInfo Update Susscessful!"
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest(new simpleResponseDto
                {
                    Success = false,
                    Message = "UserInfo Update Error!"
                });
            }
        }
    }
   
}
