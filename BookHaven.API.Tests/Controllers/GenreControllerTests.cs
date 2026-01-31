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
    public class GenreControllerTests : IDisposable
    {
        private readonly BookHavenDbContext _context;
        private readonly GenreController _controller;

        public GenreControllerTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<BookHavenDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new BookHavenDbContext(options);
            _controller = new GenreController(_context);

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            var testGenres = new List<GenreInfo>
            {
                new GenreInfo
                {
                    Id = 1,
                    Name = "Fantasy",
                    Description = "Magical worlds and adventures"
                },
                new GenreInfo
                {
                    Id = 2,
                    Name = "Science Fiction",
                    Description = "Future worlds and technology"
                }
            };

            _context.Genres.AddRange(testGenres);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        #region Read Tests

        [Fact]
        public async Task Read_WithValidId_ReturnsOkResultWithGenre()
        {
            // Act
            var result = await _controller.Read(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedGenre = Assert.IsType<GenreInfo>(okResult.Value);
            Assert.Equal(1, returnedGenre.Id);
            Assert.Equal("Fantasy", returnedGenre.Name);
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
        public async Task ReadAll_ReturnsOkResultWithAllGenres()
        {
            // Act
            var result = await _controller.ReadAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedGenres = Assert.IsType<List<GenreInfo>>(okResult.Value);
            Assert.Equal(2, returnedGenres.Count);
        }

        [Fact]
        public async Task ReadAll_WithNoGenres_ReturnsEmptyList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BookHavenDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var emptyContext = new BookHavenDbContext(options);
            var emptyController = new GenreController(emptyContext);

            // Act
            var result = await emptyController.ReadAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedGenres = Assert.IsType<List<GenreInfo>>(okResult.Value);
            Assert.Empty(returnedGenres);

            emptyContext.Dispose();
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_WithValidGenre_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var newGenre = new GenreInfo
            {
                Name = "Mystery",
                Description = "Intriguing puzzles and secrets"
            };

            // Act
            var result = await _controller.Create(newGenre);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(GenreController.Read), createdResult.ActionName);
            Assert.NotNull(createdResult.Value);
        }

        [Fact]
        public async Task Create_WithValidGenre_PersistsToDatabase()
        {
            // Arrange
            var newGenre = new GenreInfo
            {
                Name = "Romance",
                Description = "Love stories and relationships"
            };

            // Act
            await _controller.Create(newGenre);

            // Assert
            var genreInDb = await _context.Genres.FirstOrDefaultAsync(g => g.Name == "Romance");
            Assert.NotNull(genreInDb);
            Assert.Equal("Romance", genreInDb.Name);
            Assert.Equal("Love stories and relationships", genreInDb.Description);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_WithValidIdAndData_ReturnsOkResult()
        {
            // Arrange
            var updateGenre = new GenreInfo
            {
                Name = "Fantasy (Updated)",
                Description = "Updated magical worlds and adventures"
            };

            // Act
            var result = await _controller.Update(1, updateGenre);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Update_WithValidId_ModifiesGenreInDatabase()
        {
            // Arrange
            var updateGenre = new GenreInfo
            {
                Name = "Science Fiction (Revised)",
                Description = "Revised future worlds and cutting-edge technology"
            };

            // Act
            await _controller.Update(2, updateGenre);

            // Assert
            var genreInDb = await _context.Genres.FindAsync(2);
            Assert.NotNull(genreInDb);
            Assert.Equal("Science Fiction (Revised)", genreInDb.Name);
            Assert.Equal("Revised future worlds and cutting-edge technology", genreInDb.Description);
        }

        [Fact]
        public async Task Update_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var updateGenre = new GenreInfo
            {
                Name = "Nonexistent Genre",
                Description = "This genre does not exist"
            };

            // Act
            var result = await _controller.Update(999, updateGenre);

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
        public async Task Delete_WithValidId_RemovesGenreFromDatabase()
        {
            // Act
            await _controller.Delete(2);

            // Assert
            var genreInDb = await _context.Genres.FindAsync(2);
            Assert.Null(genreInDb);
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