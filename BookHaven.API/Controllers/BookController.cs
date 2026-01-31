using Microsoft.AspNetCore.Mvc;
using BookHaven.API.Data;
using BookHaven.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookHaven.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly BookHavenDbContext _context;

        public BookController(BookHavenDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Read - Get a book by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Read(int id)
        {
            var book = await _context.Books.FindAsync(id);
            
            if (book == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found." });
            }

            return Ok(book);
        }

        /// <summary>
        /// Read All - Get all books with pagination and search support
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ReadAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 6, [FromQuery] string search = "")
        {
            // Validate pagination parameters
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 6;
            if (pageSize > 50) pageSize = 50; // Max 50 items per page

            // Build query
            IQueryable<BookInfo> query = _context.Books;

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLower().Trim();
                query = query.Where(b => 
                    b.Title.ToLower().Contains(searchTerm) ||
                    b.Description.ToLower().Contains(searchTerm) ||
                    b.ISBN.ToLower().Contains(searchTerm)
                );
            }

            var totalBooks = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalBooks / (double)pageSize);

            var books = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new
            {
                pageNumber,
                pageSize,
                totalBooks,
                totalPages,
                searchTerm = search,
                data = books
            };

            return Ok(response);
        }

        /// <summary>
        /// Search - Get books by title or author with author information
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 6)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { message = "Search query is required." });
            }

            var searchTerm = query.ToLower().Trim();

            // Get matching books
            var matchingBooks = await _context.Books
                .Where(b => b.Title.ToLower().Contains(searchTerm) ||
                           b.Description.ToLower().Contains(searchTerm) ||
                           b.ISBN.ToLower().Contains(searchTerm))
                .ToListAsync();

            // Get all authors for enrichment
            var authors = await _context.Authors.ToListAsync();

            // Enrich books with author names
            var enrichedBooks = matchingBooks.Select(b => new
            {
                b.Id,
                b.Title,
                b.AuthorId,
                AuthorName = authors.FirstOrDefault(a => a.Id == b.AuthorId)?.Name ?? "Unknown",
                b.GenreId,
                b.PublishedDate,
                b.Description,
                b.Price,
                b.ISBN,
                b.StockQuantity
            }).ToList();

            var totalBooks = enrichedBooks.Count;
            var totalPages = (int)Math.Ceiling(totalBooks / (double)pageSize);

            var paginatedBooks = enrichedBooks
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new
            {
                pageNumber,
                pageSize,
                totalBooks,
                totalPages,
                searchQuery = query,
                data = paginatedBooks
            };

            return Ok(response);
        }

        /// <summary>
        /// Create - Add a new book to the database
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookInfo bookInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Books.Add(bookInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Read), new { id = bookInfo.Id }, bookInfo);
        }

        /// <summary>
        /// Update - Modify an existing book
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookInfo bookInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingBook = await _context.Books.FindAsync(id);
            
            if (existingBook == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found." });
            }

            existingBook.Title = bookInfo.Title;
            existingBook.AuthorId = bookInfo.AuthorId;
            existingBook.GenreId = bookInfo.GenreId;
            existingBook.PublishedDate = bookInfo.PublishedDate;
            existingBook.Description = bookInfo.Description;
            existingBook.Price = bookInfo.Price;
            existingBook.ISBN = bookInfo.ISBN;
            existingBook.StockQuantity = bookInfo.StockQuantity;

            _context.Books.Update(existingBook);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Book updated successfully.", book = existingBook });
        }

        /// <summary>
        /// Delete - Remove a book from the database
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            
            if (book == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found." });
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Book deleted successfully." });
        }
    }
}
