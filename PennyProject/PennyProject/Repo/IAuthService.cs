using PennyProject.Models;

namespace PennyProject.Repo
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto model);
        Task<UserInfoDto> GetUserInfoAsync(string userId);
        Task<bool> UpdateUserInfoAsync(UserUpdateRequestDto model);
    }
}
