using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private new List<String> _allowedExtentions = new List<string> { ".jpg,.png" };
        private long _maxAllowedPosterSize = 1024 * 1024;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GeatAllAsync()
        {
            var movies = await _context.Movies.Include(m => m.Genre).ToListAsync();

            return Ok(movies);
        }
        [HttpGet(template:"{id}")] 
        public async Task<IActionResult>GetAsync(int id)
        {
            var movie = await _context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m=>m.Id==id);
            if(movie == null)
            {
                return NotFound();
            }
            return Ok(movie);
        }
        [HttpGet("GetByGenreId/{genreId}")]
        public async Task<IActionResult> GetByGenreIdAsync(byte? genreId)
        {
            
            var movies = await _context.Movies

                .Where(m => m.GenreId == genreId)
                .OrderByDescending(m => m.Rate)
                .Include(m => m.Genre)
                .ToListAsync();
            if(!movies.Any())
            {
                return BadRequest("error");
            }

            return Ok(movies);
        }



        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm]MovieDto dto)
        {
            if(dto.poster == null)
            {
                return BadRequest("Poster is required");
            }
            if (!_allowedExtentions.Contains(Path.GetExtension(dto.poster.FileName).ToLower()))
            {
                return BadRequest(error: "Max Allowed size for poster is 1MB");
            }
            if(dto.poster.Length > _maxAllowedPosterSize)
            {
                return BadRequest(error: "only .jpg , .png are allowed");

            }
            var isValidGenre = await _context.Genres.AnyAsync(g=>g.Id==dto.GenreId);
            if (!isValidGenre)
            {
                return BadRequest(error: "Invalid GenreId");

            }
            using var datastream = new MemoryStream();
            await dto.poster.CopyToAsync(datastream);
            var movie = new Movie
            {
                GenreId = dto.GenreId,
                Title = dto.Title,
                poster = datastream.ToArray(),
                Rate = dto.Rate,
                Storeline = dto.Storeline, 
                Year = dto.Year
            };
            await _context.AddAsync(movie);
            _context.SaveChanges();
            return Ok(movie);
        }

        [HttpDelete(template:"{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _context.Movies.SingleOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound(value: $"No Movie was found with ID:{id}");
            }
            _context.Remove(id);
            _context.SaveChanges();
            return Ok(movie);
        }

        [HttpPut(template:"{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm]MovieDto dto)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound(value: $"No Movie was found with ID:{id}");
            }
            var isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!isValidGenre)
            {
                return BadRequest(error: "Invalid GenreId");

            }
            if(dto.poster != null)
            {
                if (!_allowedExtentions.Contains(Path.GetExtension(dto.poster.FileName).ToLower()))
                {
                    return BadRequest(error: "Max Allowed size for poster is 1MB");
                }
                if (dto.poster.Length > _maxAllowedPosterSize)
                {
                    return BadRequest(error: "only .jpg , .png are allowed");

                }
                using var datastream = new MemoryStream();
                await dto.poster.CopyToAsync(datastream);
                movie.poster = datastream.ToArray();
            }
            movie.Title = dto.Title;
            movie.GenreId = dto.GenreId;
            movie.Year = dto.Year;
            movie.Storeline = dto.Storeline;
            movie.Rate = dto.Rate;
            _context.SaveChanges();
            return Ok(movie);
        }
        
    }
}
