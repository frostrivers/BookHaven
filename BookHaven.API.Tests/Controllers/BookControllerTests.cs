using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookHaven.API.Controllers;
using BookHaven.API.Data;
using BookHaven.API.Models;
using Moq;

namespace BookHaven.API.Tests.Controllers
{
    public class BookControllerTests : IDisposable
    {
        private readonly BookHavenDbContext _context;
        private readonly BookController _controller;

        public BookControllerTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<BookHavenDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new BookHavenDbContext(options);
            _controller = new BookController(_context);

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            var testBooks = new List<BookInfo>
            {
                new BookInfo
                {
                    Id = 1,
                    Title = "The Hobbit",
                    AuthorId = 1,
                    GenreId = 1,
                    PublishedDate = new DateTime(1937, 9, 21),
                    Description = "A fantasy adventure",
                    Price = 15.99m,
                    ISBN = "9780547928227",
                    StockQuantity = 25
                },
                new BookInfo
                {
                    Id = 2,
                    Title = "I, Robot",
                    AuthorId = 2,
                    GenreId = 2,
                    PublishedDate = new DateTime(1950, 12, 2),
                    Description = "A science fiction classic",
                    Price = 12.99m,
                    ISBN = "9780553294385",
                    StockQuantity = 30
                }
            };

            _context.Books.AddRange(testBooks);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        #region Read Tests

        [Fact]
        public async Task Read_WithValidId_ReturnsOkResultWithBook()
        {
            // Act
            var result = await _controller.Read(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBook = Assert.IsType<BookInfo>(okResult.Value);
            Assert.Equal(1, returnedBook.Id);
            Assert.Equal("The Hobbit", returnedBook.Title);
        }

        [Fact]
        public async Task Read_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Read(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task ReadAll_ReturnsOkResultWithAllBooks()
        {
            // Act
            var result = await _controller.ReadAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBooks = Assert.IsType<List<BookInfo>>(okResult.Value);
            Assert.Equal(2, returnedBooks.Count);
        }

        [Fact]
        public async Task ReadAll_WithNoBooks_ReturnsEmptyList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BookHavenDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var emptyContext = new BookHavenDbContext(options);
            var emptyController = new BookController(emptyContext);

            // Act
            var result = await emptyController.ReadAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBooks = Assert.IsType<List<BookInfo>>(okResult.Value);
            Assert.Empty(returnedBooks);

            emptyContext.Dispose();
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_WithValidBook_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var newBook = new BookInfo
            {
                Title = "Dune",
                AuthorId = 3,
                GenreId = 2,
                PublishedDate = new DateTime(1965, 6, 1),
                Description = "Epic science fiction",
                Price = 18.99m,
                ISBN = "9780441172719",
                StockQuantity = 40
            };

            // Act
            var result = await _controller.Create(newBook);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(BookController.Read), createdResult.ActionName);
            Assert.NotNull(createdResult.Value);
        }

        [Fact]
        public async Task Create_WithValidBook_PersistsToDatabase()
        {
            // Arrange
            var newBook = new BookInfo
            {
                Title = "1984",
                AuthorId = 4,
                GenreId = 3,
                PublishedDate = new DateTime(1949, 6, 8),
                Description = "Dystopian novel",
                Price = 13.99m,
                ISBN = "9780451524935",
                StockQuantity = 50
            };

            // Act
            await _controller.Create(newBook);

            // Assert
            var bookInDb = await _context.Books.FirstOrDefaultAsync(b => b.Title == "1984");
            Assert.NotNull(bookInDb);
            Assert.Equal("1984", bookInDb.Title);
            Assert.Equal(4, bookInDb.AuthorId);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_WithValidIdAndData_ReturnsOkResult()
        {
            // Arrange
            var updateBook = new BookInfo
            {
                Title = "The Hobbit: Updated",
                AuthorId = 1,
                GenreId = 1,
                PublishedDate = new DateTime(1937, 9, 21),
                Description = "Updated description",
                Price = 17.99m,
                ISBN = "9780547928227",
                StockQuantity = 35
            };

            // Act
            var result = await _controller.Update(1, updateBook);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Update_WithValidId_ModifiesBookInDatabase()
        {
            // Arrange
            var updateBook = new BookInfo
            {
                Title = "The Hobbit: Revised Edition",
                AuthorId = 1,
                GenreId = 1,
                PublishedDate = new DateTime(1937, 9, 21),
                Description = "Revised description",
                Price = 18.99m,
                ISBN = "9780547928227",
                StockQuantity = 40
            };

            // Act
            await _controller.Update(1, updateBook);

            // Assert
            var bookInDb = await _context.Books.FindAsync(1);
            Assert.NotNull(bookInDb);
            Assert.Equal("The Hobbit: Revised Edition", bookInDb.Title);
            Assert.Equal(18.99m, bookInDb.Price);
            Assert.Equal(40, bookInDb.StockQuantity);
        }

        [Fact]
        public async Task Update_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var updateBook = new BookInfo
            {
                Title = "Nonexistent Book",
                AuthorId = 1,
                GenreId = 1,
                PublishedDate = DateTime.Now,
                Price = 10.99m,
                StockQuantity = 10
            };

            // Act
            var result = await _controller.Update(999, updateBook);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_WithValidId_ReturnsOkResult()
        {
            // Act
            var result = await _controller.Delete(2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Delete_WithValidId_RemovesBookFromDatabase()
        {
            // Act
            await _controller.Delete(2);

            // Assert
            var bookInDb = await _context.Books.FindAsync(2);
            Assert.Null(bookInDb);
        }

        [Fact]
        public async Task Delete_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion
    }
}