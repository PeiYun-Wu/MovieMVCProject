using Microsoft.EntityFrameworkCore;
using PennyProject.DataBase.MovieDB;
using PennyProject.Models;

namespace PennyProject.Repo
{
    public class FavService : IFavService
    {
        private readonly PennyMovieDBContext _dbContext;
        private readonly ILogger<FavService> _logger;

        public FavService(PennyMovieDBContext dbContext, ILogger<FavService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<FavoriteMovieDto>> GetUserFavoritesAsync(string userId)
        {
            var favoriteMovies = await _dbContext.MovieInfos
              .Join(_dbContext.UserFavorites.Where(f => f.MemberId == userId),
                    m => m.MovieId,
                    f => f.MovieId,
                    (m, f) => new FavoriteMovieDto
                    {
                        MovieId = m.MovieId,
                        MovieChinessName = m.MovieChinessName,
                        ImgName = m.ImgName,
                        ReleaseDateTime = m.ReleaseDateTime,

                    })
              .OrderByDescending(m => m.ReleaseDateTime)
              .ToListAsync();

            return favoriteMovies;

        }

        public async Task<ApiResponseDto> AddFavoriteAsync(string userId, string movieId)
        {

            var movieInfo = await _dbContext.MovieInfos.FirstOrDefaultAsync(m => m.MovieId == movieId);
            var existingFavorite = await _dbContext.UserFavorites
              .FirstOrDefaultAsync(f => f.MemberId == userId && f.MovieId == movieId);

            if (existingFavorite == null)
            {
                var addInfo = new UserFavorite
                {
                    MovieId = movieId,
                    MemberId = userId,
                    MovieChinessName = movieInfo.MovieChinessName,
                    CreateDateTime = DateTime.Now,
                };
                _dbContext.UserFavorites.Add(addInfo);
                await _dbContext.SaveChangesAsync();
                _logger.LogDebug($"Added favorite movie: {movieId} for user: {userId}");
            }

            return (new ApiResponseDto
            {
                Success = true,
                Message = "add success! "
            });


        }

        public async Task<ApiResponseDto> RemoveFavoriteAsync(string userId, string movieId)
        {
            var favorite = await _dbContext.UserFavorites
                  .FirstOrDefaultAsync(f => f.MemberId == userId && f.MovieId == movieId);

            if (favorite == null)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "can't find this record!"
                };
            }

            _dbContext.UserFavorites.Remove(favorite);

            await _dbContext.SaveChangesAsync();

            _logger.LogDebug($"Removed favorite movie: {movieId} for user: {userId}");
            return new ApiResponseDto
            {
                Success = true,
                Message = "remove success!"
            };
        }

    }
}
