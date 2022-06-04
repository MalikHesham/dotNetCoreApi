
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : Controller
    {

        private readonly ApplicationDbContext _context;

        private new List<string> _allowedExtensions = new List<string> {".jpg",".png"};

        private const long _MAX_POSTER_IMAGE_SIZE = 1572864; // 1.5 megabyte

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {
            if(!_allowedExtensions.Contains(Path.GetExtension(dto.PosterUrl.FileName).ToLower())){
                return BadRequest("Only png and jpg extensions are allowed!");
            }

            if(dto.PosterUrl.Length > _MAX_POSTER_IMAGE_SIZE){
                return BadRequest("Only file sizes of 1.5MB or less are allowed");
            }

            using var dataStream = new MemoryStream();
            await dto.PosterUrl.CopyToAsync(dataStream);

            var movie = new Movie{
                GenreId = dto.GenreId,
                Title = dto.Title,
                PosterUrl = dataStream.ToArray(),
                Rate = dto.Rate,
                StoryLine = dto.StoryLine,
                Year = dto.Year
            };
            
            await _context.AddAsync(movie);
            _context.SaveChanges();
            return Ok(movie);
        }


    }
}