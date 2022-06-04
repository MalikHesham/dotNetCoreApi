namespace WebAPI.Dtos
{
    public class MovieDto
    {
        [MaxLength(250)]
        public string Title {get; set;}

        public double Rate {get; set;}

        [MaxLength(2500)]
        public string StoryLine {get; set;}
        public int Year {get; set;}

        public IFormFile PosterUrl {get; set;}

        public byte GenreId {get; set;}
    }
}