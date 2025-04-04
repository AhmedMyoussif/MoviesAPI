using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Models;


namespace MoviesAPI.Controllers
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
            var generes = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
            return Ok(generes);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateGenreDto dto)
        {
            var genre = new Genre
            {
                Name = dto.Name,
            };
            await _context.Genres.AddAsync(genre);
            _context.SaveChanges();
            return Ok(genre);
        }
        [HttpPut(template:"{Id}")]
        public async Task<IActionResult> UpdateAsync(int id , [FromBody] Genre dto)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if(genre ==null)
            {
                return NotFound(value: $"No genre was found with id :{id}");
            }
            genre.Name = dto.Name;
            _context.SaveChanges();
            return Ok(genre);
        }

        [HttpDelete(template: "{Id}")]

        public async Task<IActionResult> DeleteAsync(int id)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if (genre == null)
            {
                return NotFound(value: $"No genre was found with id :{id}");
            }
            _context.Remove(genre);
            _context.SaveChanges();
            return Ok(genre);
        }
    }
}
