using Microsoft.EntityFrameworkCore;
using PennyProject.DataBase.MovieDB;
using PennyProject.Models;

namespace PennyProject.Repo
{
    public class AuthService : IAuthService
    {
        private readonly PennyMovieDBContext _dbContext;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            PennyMovieDBContext dbContext,
            ILogger<AuthService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

      
        public async Task<ApiResponseDto> LoginAsync(LoginRequestDto model)
        {
            var user = await _dbContext.UserRoles
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "Sorry... Not registered yet"
                };
            }

            if (user.Password != model.Password)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "Password Error!"
                };
            }

            _httpContextAccessor.HttpContext?.Session.SetString("UserId", user.UserId);
            _logger.LogDebug($"login user : {user.UserName}");

            return new ApiResponseDto
            {
                Success = true,
                Message = $"Login Success! .. Hi {user.UserName}",
               
            };
        }

        public async Task<UserInfoDto> GetUserInfoAsync(string userId)
        {
            var user = await _dbContext.UserRoles
                .Where(u => u.UserId == userId)
                .Select(u => new UserInfoDto
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    Age = u.Age
                })
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<bool> UpdateUserInfoAsync(UserUpdateRequestDto model)
        {
            var existingUser = await _dbContext.UserRoles
                .FirstOrDefaultAsync(u => u.UserId == model.UserId);

            if (existingUser == null)
            {
                return false;
            }

            existingUser.UserName = model.UserName;
            existingUser.Password = model.Password;
            existingUser.Email = model.Email;
            existingUser.Age = model.Age;
            existingUser.UpdateDateTime = DateTime.Now;

            await _dbContext.SaveChangesAsync();
            _logger.LogDebug($"UserName:{model.UserName}, UserInfo Update Successful!");

            return true;
        }

    }
}
