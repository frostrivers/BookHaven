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
            var genre = await _context.ItemTypes.FindAsync(id);
            
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
            var genres = await _context.ItemTypes.ToListAsync();
            return Ok(genres);
        }

        /// <summary>
        /// Create - Add a new genre to the database
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ItemTypeInfo itemTypeInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ItemTypes.Add(itemTypeInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Read), new { id = itemTypeInfo.Id }, itemTypeInfo);
        }

        /// <summary>
        /// Update - Modify an existing genre
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ItemTypeInfo itemTypeInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingGenre = await _context.ItemTypes.FindAsync(id);
            
            if (existingGenre == null)
            {
                return NotFound(new { message = $"Genre with ID {id} not found." });
            }

            existingGenre.Name = itemTypeInfo.Name;
            existingGenre.Description = itemTypeInfo.Description;

            _context.ItemTypes.Update(existingGenre);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Genre updated successfully.", genre = existingGenre });
        }

        /// <summary>
        /// Delete - Remove a genre from the database
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _context.ItemTypes.FindAsync(id);
            
            if (genre == null)
            {
                return NotFound(new { message = $"Genre with ID {id} not found." });
            }

            _context.ItemTypes.Remove(genre);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Genre deleted successfully." });
        }
    }
}