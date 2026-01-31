using Microsoft.AspNetCore.Mvc;
using BookHaven.API.Data;
using BookHaven.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookHaven.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : ControllerBase
    {
        private readonly BookHavenDbContext _context;

        public GenreController(BookHavenDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Read - Get a genre by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            
            if (genre == null)
            {
                return NotFound(new { message = $"Genre with ID {id} not found." });
            }

            return Ok(genre);
        }

        /// <summary>
        /// Read All - Get all genres
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ReadAll()
        {
            var genres = await _context.Genres.ToListAsync();
            return Ok(genres);
        }

        /// <summary>
        /// Create - Add a new genre to the database
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GenreInfo genreInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Genres.Add(genreInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Read), new { id = genreInfo.Id }, genreInfo);
        }

        /// <summary>
        /// Update - Modify an existing genre
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] GenreInfo genreInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingGenre = await _context.Genres.FindAsync(id);
            
            if (existingGenre == null)
            {
                return NotFound(new { message = $"Genre with ID {id} not found." });
            }

            existingGenre.Name = genreInfo.Name;
            existingGenre.Description = genreInfo.Description;

            _context.Genres.Update(existingGenre);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Genre updated successfully.", genre = existingGenre });
        }

        /// <summary>
        /// Delete - Remove a genre from the database
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            
            if (genre == null)
            {
                return NotFound(new { message = $"Genre with ID {id} not found." });
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Genre deleted successfully." });
        }
    }
}