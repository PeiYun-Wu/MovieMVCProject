using PennyProject.Models;

namespace PennyProject.Repo
{
    public interface IFavService
    {
        Task<List<FavoriteMovieDto>> GetUserFavoritesAsync(string memberId);
        Task<ApiResponseDto> AddFavoriteAsync(string userId, string movieId);
        Task<ApiResponseDto> RemoveFavoriteAsync(string memberId, string movieId);
    }
}
