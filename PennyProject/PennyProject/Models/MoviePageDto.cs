namespace PennyProject.Models
{
    public class MoviePageDto
    {
        public class MoviesByCountryDto
        {
            public string Country { get; set; }
            public List<MovieDto> Movies { get; set; }
        }

        public class MovieDto
        {
            public string MovieId { get; set; }
            public string Description { get; set; }
            public string Country { get; set; }
            public DateTime? ReleaseDateTime { get; set; }
            public bool IsFavorite { get; set; }
            public string MovieChinessName { get; set; }
            public string MovieImgName { get; set; }
        }

        public class HomePageDto
        {
            public Dictionary<string, List<MovieDto>> MoviesByCountry { get; set; }
            public List<string> UserFavorites { get; set; }
        }
    }
}
