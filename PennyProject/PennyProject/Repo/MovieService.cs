using Microsoft.EntityFrameworkCore;
using PennyProject.DataBase.MovieDB;
using static PennyProject.Models.Enum.CountryOrderEnum;
using static PennyProject.Models.MoviePageDto;

namespace PennyProject.Repo
{
    public class MovieService : IMovieService
    {
        private readonly PennyMovieDBContext _dbContext;
        private readonly ILogger<MovieService> _logger;

        public MovieService(PennyMovieDBContext dbContext, ILogger<MovieService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<HomePageDto> GetHomePageDataAsync(string userId)
        {
            try
            {
                // get user fav
                var userFavorites = await _dbContext.UserFavorites
                    .Where(f => f.MemberId == userId)
                    .Select(f => f.MovieId)
                    .ToListAsync();

                // get all movie
                var movies = await _dbContext.MovieInfos
                    .Select(m => new MovieDto
                    {
                        MovieId = m.MovieId,
                        Description = m.Description,
                        Country = m.Country,
                        ReleaseDateTime = m.ReleaseDateTime,
                        IsFavorite = userFavorites.Contains(m.MovieId),
                        MovieChinessName = m.MovieChinessName,
                        MovieImgName = m.ImgName
                    })
                    .ToListAsync();

                // group 
                var moviesByCountry = movies
                    .GroupBy(m => m.Country)
                    .OrderBy(g => GetCountryOrder(g.Key))
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderByDescending(m => m.ReleaseDateTime).ToList()
                    );

                return new HomePageDto
                {
                    MoviesByCountry = moviesByCountry,
                    UserFavorites = userFavorites
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting home page data");
                throw;
            }
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
    }
}
