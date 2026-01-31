using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookHaven.API.Controllers;
using BookHaven.API.Data;
using BookHaven.API.Models;

namespace BookHaven.API.Tests.Controllers
{
    public class AuthorControllerTests : IDisposable
    {
        private readonly BookHavenDbContext _context;
        private readonly AuthorController _controller;

        public AuthorControllerTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<BookHavenDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new BookHavenDbContext(options);
            _controller = new AuthorController(_context);

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            var testAuthors = new List<AuthorInfo>
            {
                new AuthorInfo
                {
                    Id = 1,
                    Name = "J.R.R. Tolkien",
                    BirthDate = new DateTime(1892, 1, 3),
                    Biography = "English author and philologist, famous for The Lord of the Rings"
                },
                new AuthorInfo
                {
                    Id = 2,
                    Name = "Isaac Asimov",
                    BirthDate = new DateTime(1920, 1, 2),
                    Biography = "American writer and professor of biochemistry, prolific science fiction author"
                }
            };

            _context.Authors.AddRange(testAuthors);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        #region Read Tests

        [Fact]
        public async Task Read_WithValidId_ReturnsOkResultWithAuthor()
        {
            // Act
            var result = await _controller.Read(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAuthor = Assert.IsType<AuthorInfo>(okResult.Value);
            Assert.Equal(1, returnedAuthor.Id);
            Assert.Equal("J.R.R. Tolkien", returnedAuthor.Name);
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
        public async Task ReadAll_ReturnsOkResultWithAllAuthors()
        {
            // Act
            var result = await _controller.ReadAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAuthors = Assert.IsType<List<AuthorInfo>>(okResult.Value);
            Assert.Equal(2, returnedAuthors.Count);
        }

        [Fact]
        public async Task ReadAll_WithNoAuthors_ReturnsEmptyList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BookHavenDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var emptyContext = new BookHavenDbContext(options);
            var emptyController = new AuthorController(emptyContext);

            // Act
            var result = await emptyController.ReadAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAuthors = Assert.IsType<List<AuthorInfo>>(okResult.Value);
            Assert.Empty(returnedAuthors);

            emptyContext.Dispose();
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_WithValidAuthor_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var newAuthor = new AuthorInfo
            {
                Name = "George R.R. Martin",
                BirthDate = new DateTime(1948, 9, 20),
                Biography = "American novelist known for A Song of Ice and Fire series"
            };

            // Act
            var result = await _controller.Create(newAuthor);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(AuthorController.Read), createdResult.ActionName);
            Assert.NotNull(createdResult.Value);
        }

        [Fact]
        public async Task Create_WithValidAuthor_PersistsToDatabase()
        {
            // Arrange
            var newAuthor = new AuthorInfo
            {
                Name = "J.K. Rowling",
                BirthDate = new DateTime(1965, 7, 31),
                Biography = "British author known for the Harry Potter series"
            };

            // Act
            await _controller.Create(newAuthor);

            // Assert
            var authorInDb = await _context.Authors.FirstOrDefaultAsync(a => a.Name == "J.K. Rowling");
            Assert.NotNull(authorInDb);
            Assert.Equal("J.K. Rowling", authorInDb.Name);
            Assert.Equal(new DateTime(1965, 7, 31), authorInDb.BirthDate);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_WithValidIdAndData_ReturnsOkResult()
        {
            // Arrange
            var updateAuthor = new AuthorInfo
            {
                Name = "J.R.R. Tolkien (Updated)",
                BirthDate = new DateTime(1892, 1, 3),
                Biography = "Updated biography"
            };

            // Act
            var result = await _controller.Update(1, updateAuthor);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Update_WithValidId_ModifiesAuthorInDatabase()
        {
            // Arrange
            var updateAuthor = new AuthorInfo
            {
                Name = "Isaac Asimov (Revised)",
                BirthDate = new DateTime(1920, 1, 2),
                Biography = "Revised biography of the prolific science fiction author"
            };

            // Act
            await _controller.Update(2, updateAuthor);

            // Assert
            var authorInDb = await _context.Authors.FindAsync(2);
            Assert.NotNull(authorInDb);
            Assert.Equal("Isaac Asimov (Revised)", authorInDb.Name);
            Assert.Equal("Revised biography of the prolific science fiction author", authorInDb.Biography);
        }

        [Fact]
        public async Task Update_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var updateAuthor = new AuthorInfo
            {
                Name = "Nonexistent Author",
                BirthDate = DateTime.Now,
                Biography = "This author does not exist"
            };

            // Act
            var result = await _controller.Update(999, updateAuthor);

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
        public async Task Delete_WithValidId_RemovesAuthorFromDatabase()
        {
            // Act
            await _controller.Delete(2);

            // Assert
            var authorInDb = await _context.Authors.FindAsync(2);
            Assert.Null(authorInDb);
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