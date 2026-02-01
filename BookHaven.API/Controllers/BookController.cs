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
            var book = await _context.SellItems.FindAsync(id);
            
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
            IQueryable<SellItemInfo> query = _context.SellItems;

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

            // Get all authors for enrichment
            var authors = await _context.Authors.ToListAsync();
            var itemTypes = await _context.ItemTypes.ToListAsync();

            // Enrich books with author names
            var enrichedBooks = books.Select(b => new
            {
                b.Id,
                b.Title,
                b.AuthorId,
                AuthorName = authors.FirstOrDefault(a => a.Id == b.AuthorId)?.Name ?? "Unknown",
                ItemTypeName = itemTypes.FirstOrDefault(a => a.Id == b.ItemTypeId)?.Name ?? "Unknown",
                b.PublishedDate,
                b.Description,
                b.Price,
                b.ISBN,
                b.StockQuantity,
                b.CoverImage
            }).ToList();

            var response = new
            {
                pageNumber,
                pageSize,
                totalBooks,
                totalPages,
                searchTerm = search,
                data = enrichedBooks
            };

            return Ok(response);
        }
        /// <summary>
        /// Search - Get books by title, author name, item type name, or ISBN with author information
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 6)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { message = "Search query is required." });
            }

            var searchTerm = query.ToLower().Trim();

            // Get matching author IDs from the database
            var matchingAuthorIds = await _context.Authors
                .Where(a => a.Name.ToLower().Contains(searchTerm))
                .Select(a => a.Id)
                .ToListAsync();

            // Get matching ItemType IDs from the database
            var matchingItemTypeIds = await _context.ItemTypes
                .Where(it => it.Name.ToLower().Contains(searchTerm))
                .Select(it => it.Id)
                .ToListAsync();

            // Get matching books with ItemType name - all filtering happens at the database level
            var matchingBooks = await _context.SellItems
                .Where(b => b.Title.ToLower().Contains(searchTerm) ||
                           b.Description.ToLower().Contains(searchTerm) ||
                           b.ISBN.ToLower().Contains(searchTerm) ||
                           matchingAuthorIds.Contains(b.AuthorId) ||
                           matchingItemTypeIds.Contains(b.ItemTypeId))
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.AuthorId,
                    b.ItemTypeId,
                    ItemTypeName = _context.ItemTypes
                        .Where(it => it.Id == b.ItemTypeId)
                        .Select(it => it.Name)
                        .FirstOrDefault() ?? $"Item Type {b.ItemTypeId}",
                    b.PublishedDate,
                    b.Description,
                    b.Price,
                    b.ISBN,
                    b.StockQuantity,
                    b.CoverImage
                })
                .ToListAsync();

            // Get all authors for enrichment
            var authors = await _context.Authors.ToListAsync();

            // Get all item types for enrichment
            var itemTypes = await _context.ItemTypes.ToListAsync();

            // Enrich books with author names
            var enrichedBooks = matchingBooks.Select(b => new
            {
                b.Id,
                b.Title,
                b.AuthorId,
                AuthorName = authors.FirstOrDefault(a => a.Id == b.AuthorId)?.Name ?? "Unknown",
                ItemTypeName = itemTypes.FirstOrDefault(a => a.Id == b.ItemTypeId)?.Name ?? "Unknown",
                b.PublishedDate,
                b.Description,
                b.Price,
                b.ISBN,
                b.StockQuantity,
                b.CoverImage
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
        public async Task<IActionResult> Create([FromBody] SellItemInfo sellItemInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SellItems.Add(sellItemInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Read), new { id = sellItemInfo.Id }, sellItemInfo);
        }

        /// <summary>
        /// Update - Modify an existing book
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SellItemInfo sellItemInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingBook = await _context.SellItems.FindAsync(id);
            
            if (existingBook == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found." });
            }

            existingBook.Title = sellItemInfo.Title;
            existingBook.AuthorId = sellItemInfo.AuthorId;
            existingBook.ItemTypeId = sellItemInfo.ItemTypeId;
            existingBook.PublishedDate = sellItemInfo.PublishedDate;
            existingBook.Description = sellItemInfo.Description;
            existingBook.Price = sellItemInfo.Price;
            existingBook.ISBN = sellItemInfo.ISBN;
            existingBook.StockQuantity = sellItemInfo.StockQuantity;

            _context.SellItems.Update(existingBook);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Book updated successfully.", book = existingBook });
        }

        /// <summary>
        /// Delete - Remove a book from the database
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.SellItems.FindAsync(id);
            
            if (book == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found." });
            }

            _context.SellItems.Remove(book);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Book deleted successfully." });
        }

        /// <summary>
        /// Get - Retrieve a list of book categories
        /// </summary>
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.SellItems
                .Select(s => new { id = s.ItemTypeId, name = s.ItemTypeId.ToString() })
                .Distinct()
                .OrderBy(x => x.id)
                .ToListAsync();

            return Ok(categories);
        }
    }
}
