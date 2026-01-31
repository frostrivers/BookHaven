using Microsoft.AspNetCore.Mvc;
using BookHaven.API.Data;
using BookHaven.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookHaven.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly BookHavenDbContext _context;

        public AuthorController(BookHavenDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Read - Get an author by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            
            if (author == null)
            {
                return NotFound(new { message = $"Author with ID {id} not found." });
            }

            return Ok(author);
        }

        /// <summary>
        /// Read All - Get all authors
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ReadAll()
        {
            var authors = await _context.Authors.ToListAsync();
            return Ok(authors);
        }

        /// <summary>
        /// Create - Add a new author to the database
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AuthorInfo authorInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Authors.Add(authorInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Read), new { id = authorInfo.Id }, authorInfo);
        }

        /// <summary>
        /// Update - Modify an existing author
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorInfo authorInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingAuthor = await _context.Authors.FindAsync(id);
            
            if (existingAuthor == null)
            {
                return NotFound(new { message = $"Author with ID {id} not found." });
            }

            existingAuthor.Name = authorInfo.Name;
            existingAuthor.BirthDate = authorInfo.BirthDate;
            existingAuthor.Biography = authorInfo.Biography;

            _context.Authors.Update(existingAuthor);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Author updated successfully.", author = existingAuthor });
        }

        /// <summary>
        /// Delete - Remove an author from the database
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            
            if (author == null)
            {
                return NotFound(new { message = $"Author with ID {id} not found." });
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Author deleted successfully." });
        }
    }
}