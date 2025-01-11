using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PennyProject.DataBase.MovieDB;
using PennyProject.Models;
using PennyProject.Repo;


namespace PennyProject.Controllers
{
  
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly PennyMovieDBContext _dbcontext;

        public AuthController(ILogger<AuthController> logger, PennyMovieDBContext dbContext , IAuthService authService)
        {
            _logger = logger;
            _dbcontext = dbContext;
            _authService = authService;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            try
            {
                var result = await _authService.LoginAsync(model);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                return StatusCode(500, new LoginResponseDto
                {
                    Success = false,
                    Message = "An error occurred during login"
                });
            }
        }

        /// <summary>
        ///  顯示會員資料
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> UserIndex()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var userInfo = await _authService.GetUserInfoAsync(userId);
            return View(userInfo);
        }

        /// <summary>
        /// 修改會員資料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("api/UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateRequestDto model)
        {
            try
            {
                var result = await _authService.UpdateUserInfoAsync(model);
                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Json(new { success = true, message = "UserInfo Update Successful!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user update");
                return BadRequest(new { success = false, message = "UserInfo Update Error!" });
            }
        }

        [HttpPost("api/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();  
            return Ok();
        }
    }
   
}
