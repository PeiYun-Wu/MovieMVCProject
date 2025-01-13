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


        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }
    }
}