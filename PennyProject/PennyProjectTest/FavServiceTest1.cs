using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PennyProject.DataBase.MovieDB;
using PennyProject.Repo;

namespace PennyProjectTest
{
    public class FavTests
    {
        private PennyMovieDBContext _dbContext;
        private FavService _favService;

        [SetUp]
        public void Setup()
        {
            //in memory db
            var dbConnTest = new DbContextOptionsBuilder<PennyMovieDBContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            _dbContext = new PennyMovieDBContext(dbConnTest);

            // logger
            var loggerFactory = new LoggerFactory();
            var logger = loggerFactory.CreateLogger<FavService>();

         
            _favService = new FavService(_dbContext, logger);

            // clean db
            _dbContext.MovieInfos.RemoveRange(_dbContext.MovieInfos);
            _dbContext.UserFavorites.RemoveRange(_dbContext.UserFavorites);
            _dbContext.SaveChanges();

        }

        [Test]
        public async Task GetUserFavoritesAsyncTest()
        {
            // Arrange
            _dbContext.MovieInfos.Add(new MovieInfo
            {
                MovieId = "movie1",
                MovieChinessName = "電影1",
                MovieEngName = "Movie 1",
                Country = "TW",                  
                ImgName = "default_image1"       
            });
            _dbContext.MovieInfos.Add(new MovieInfo
            {
                MovieId = "movie2",
                MovieChinessName = "電影2",
                MovieEngName = "Movie 2",
                Country = "TW",                   
                ImgName = "default_image2"
            });

            _dbContext.UserFavorites.Add(new UserFavorite
            {
                MovieId = "movie1",
                MemberId = "user1",
                MovieChinessName = "電影1"
            });
            _dbContext.UserFavorites.Add(new UserFavorite
            {
                MovieId = "movie2",
                MemberId = "user1",
                MovieChinessName = "電影2"
            });

            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _favService.GetUserFavoritesAsync("user1");

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("movie1", result[0].MovieId);
            Assert.AreEqual("movie2", result[1].MovieId);

        }
        [Test]
        public async Task AddFavoriteAsyncTest()
        {
            // Arrange
            _dbContext.MovieInfos.Add(new MovieInfo
            {
                MovieId = "movie3",
                MovieChinessName = "電影3",
                MovieEngName = "Movie 3",
                Country = "TW",
                ImgName = "default_image3"
            });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _favService.AddFavoriteAsync("user1", "movie3");

            // Assert
            var favorite = _dbContext.UserFavorites
                .FirstOrDefault(f => f.MemberId == "user1" && f.MovieId == "movie3");

            Assert.IsNotNull(favorite);
            Assert.AreEqual(true, result.Success);

            Assert.AreEqual("user1", favorite.MemberId);
            Assert.AreEqual("movie3", favorite.MovieId);
            Assert.AreEqual("電影3", favorite.MovieChinessName);
        }

        [Test]
        public async Task RemoveFavoriteAsyncTest()
        {
            // Arrange
            _dbContext.MovieInfos.Add(new MovieInfo
            {
                MovieId = "movie5",
                MovieChinessName = "電影5",
                MovieEngName = "Movie 5",
                Country = "TW",
                ImgName = "default_image5"
            });

            _dbContext.UserFavorites.Add(new UserFavorite
            {
                MovieId = "movie5",
                MemberId = "user1",
                MovieChinessName = "電影5",
            });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _favService.RemoveFavoriteAsync("user1", "movie5");

            // Assert
            var favorite = _dbContext.UserFavorites
                .FirstOrDefault(f => f.MemberId == "user1" && f.MovieId == "movie5");

            Assert.IsNull(favorite, "remove success");
            Assert.AreEqual(true, result.Success);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }
    }
}