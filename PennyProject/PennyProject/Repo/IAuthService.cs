using PennyProject.Models;

namespace PennyProject.Repo
{
    public interface IAuthService
    {
        Task<ApiResponseDto> LoginAsync(LoginRequestDto model);
        Task<UserInfoDto> GetUserInfoAsync(string userId);
        Task<bool> UpdateUserInfoAsync(UserUpdateRequestDto model);
    }
}
