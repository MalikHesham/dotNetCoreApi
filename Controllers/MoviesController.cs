
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public async Task<IActionResult> getAllAsync()
        {
            var movies = await _context.Movies.Include(m=>m.Genre)
                .ToListAsync();
            return Ok(
                movies
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            var requestMovie = await _context.Movies.Include(m=>m.Genre)
                .SingleOrDefaultAsync(m => m.Id == id);

            if(requestMovie == null)
                return NotFound();
                
            return Ok(
                requestMovie
            );
        }

        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreID(byte genreID)
        {
            var moviesById = await _context.Movies.Include(m=>m.Genre)
                .Where(m => m.GenreId == genreID)
                .ToListAsync();
                        
            if(moviesById == null)
                return NotFound();
                
            return Ok(
                moviesById
            );
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {
            if(dto.PosterUrl == null){
                return BadRequest("A poster is required");
            }

            if(!_allowedExtensions.Contains(Path.GetExtension(dto.PosterUrl.FileName).ToLower())){
                return BadRequest("Only png and jpg extensions are allowed!");
            }

            if(dto.PosterUrl.Length > _MAX_POSTER_IMAGE_SIZE){
                return BadRequest("Only file sizes of 1.5MB or less are allowed");
            }

            bool isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if(!isValidGenre)
                return BadRequest("The genre Id field value does not exist");
            
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int Id,[FromForm] MovieDto dto)
        {
            var movie = await _context.Movies.FindAsync(Id);
            if(movie == null )
                return NotFound($"No movie was found with ID: {Id}");

            if(dto.PosterUrl != null){
                if(!_allowedExtensions.Contains(Path.GetExtension(dto.PosterUrl.FileName).ToLower())){
                    return BadRequest("Only png and jpg extensions are allowed!");
                }

                if(dto.PosterUrl.Length > _MAX_POSTER_IMAGE_SIZE){
                    return BadRequest("Only file sizes of 1.5MB or less are allowed");
                }

                bool isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
                if(!isValidGenre)
                    return BadRequest("The genre Id field value does not exist");

                using var dataStream = new MemoryStream();
                await dto.PosterUrl.CopyToAsync(dataStream);
                movie.PosterUrl = dataStream.ToArray();
            }

            movie.Title = dto.Title;
            movie.GenreId = dto.GenreId;
            movie.Year = dto.Year;
            movie.StoryLine = dto.StoryLine;
            movie.Rate = dto.Rate;

            _context.SaveChanges();

            return Ok(
                movie
            );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int Id)
        {
            var movie = await _context.Movies.FindAsync(Id);
            if(movie == null )
                return NotFound($"No movie was found with ID: {Id}");

            _context.Remove(movie);
            _context.SaveChanges();
            
            return Ok(movie);
        }

    }
}