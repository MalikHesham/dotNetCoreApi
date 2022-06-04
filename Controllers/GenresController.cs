using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync() 
        {
            var allRecords = await _context.Genres
                .OrderBy(m=>m.Name)
                .ToListAsync();

            return Ok(allRecords);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGenreAsync(CreateGenreDto dto)
        {
            var genre = new Genre() { Name = dto.Name };
            await _context.Genres.AddAsync(genre);
            _context.SaveChanges();
            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int Id, [FromBody] CreateGenreDto dto)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g=>g.Id == Id);
            if(genre == null )
                return NotFound($"No genre was found with ID: {Id}");

            genre.Name = dto.Name;
            _context.SaveChanges();
            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(byte id)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g=>g.Id == id);
            if(genre == null )
                return NotFound($"No genre was found with ID: {id}");

            _context.Remove(genre);
            _context.SaveChanges();
            
            return Ok(genre);
        }
    }
}
