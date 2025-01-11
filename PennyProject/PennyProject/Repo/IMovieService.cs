using static PennyProject.Models.MoviePageDto;

namespace PennyProject.Repo
{
    public interface IMovieService
    {
        Task<HomePageDto> GetHomePageDataAsync(string userId);
    }
}
