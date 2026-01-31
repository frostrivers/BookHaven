using BookHaven.API.Models;

namespace BookHaven.API.Data
{
    public class DatabaseSeeder
    {
        private readonly BookHavenDbContext _context;

        public DatabaseSeeder(BookHavenDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Only seed if database is empty
            if (_context.Users.Any() || _context.Authors.Any() || 
                _context.Genres.Any() || _context.Books.Any())
            {
                return;
            }

            // Seed Genres
            var genres = new List<GenreInfo>
            {
                new GenreInfo { Name = "Fantasy", Description = "Magical worlds and adventures" },
                new GenreInfo { Name = "Science Fiction", Description = "Future worlds and technology" },
                new GenreInfo { Name = "Mystery", Description = "Intriguing puzzles and secrets" },
                new GenreInfo { Name = "Romance", Description = "Love stories and relationships" },
                new GenreInfo { Name = "Thriller", Description = "Suspenseful and action-packed" }
            };
            await _context.Genres.AddRangeAsync(genres);
            await _context.SaveChangesAsync();

            // Seed Authors
            var authors = new List<AuthorInfo>
            {
                new AuthorInfo 
                { 
                    Name = "J.R.R. Tolkien", 
                    BirthDate = new DateTime(1892, 1, 3),
                    Biography = "English author and philologist, famous for The Lord of the Rings"
                },
                new AuthorInfo 
                { 
                    Name = "Isaac Asimov", 
                    BirthDate = new DateTime(1920, 1, 2),
                    Biography = "American writer and professor of biochemistry, prolific science fiction author"
                },
                new AuthorInfo 
                { 
                    Name = "Agatha Christie", 
                    BirthDate = new DateTime(1890, 9, 15),
                    Biography = "British writer known for detective novels"
                },
                new AuthorInfo 
                { 
                    Name = "Jane Austen", 
                    BirthDate = new DateTime(1775, 12, 16),
                    Biography = "English novelist known for romantic fiction"
                },
                new AuthorInfo 
                { 
                    Name = "George R.R. Martin", 
                    BirthDate = new DateTime(1948, 9, 20),
                    Biography = "American novelist known for fantasy epics and complex narratives"
                },
                new AuthorInfo 
                { 
                    Name = "Arthur Conan Doyle", 
                    BirthDate = new DateTime(1859, 5, 22),
                    Biography = "Scottish physician and writer, creator of Sherlock Holmes"
                },
                new AuthorInfo 
                { 
                    Name = "Stephen King", 
                    BirthDate = new DateTime(1947, 9, 21),
                    Biography = "American author renowned for horror and thriller novels"
                },
                new AuthorInfo 
                { 
                    Name = "Philip K. Dick", 
                    BirthDate = new DateTime(1928, 12, 16),
                    Biography = "American science fiction author exploring reality and perception"
                }
            };
            await _context.Authors.AddRangeAsync(authors);
            await _context.SaveChangesAsync();

            // Seed Books
            var books = new List<BookInfo>
            {
                new BookInfo 
                { 
                    Title = "The Lord of the Rings",
                    AuthorId = 1,
                    GenreId = 1,
                    PublishedDate = new DateTime(1954, 7, 29),
                    Description = "An epic fantasy adventure",
                    Price = 29.99m,
                    ISBN = "9780544003415",
                    StockQuantity = 50
                },
                new BookInfo 
                { 
                    Title = "Foundation",
                    AuthorId = 2,
                    GenreId = 2,
                    PublishedDate = new DateTime(1951, 6, 1),
                    Description = "A science fiction masterpiece",
                    Price = 18.99m,
                    ISBN = "9780553293357",
                    StockQuantity = 35
                },
                new BookInfo 
                { 
                    Title = "Murder on the Orient Express",
                    AuthorId = 3,
                    GenreId = 3,
                    PublishedDate = new DateTime(1934, 1, 1),
                    Description = "A classic murder mystery",
                    Price = 14.99m,
                    ISBN = "9780062073556",
                    StockQuantity = 45
                },
                new BookInfo 
                { 
                    Title = "Pride and Prejudice",
                    AuthorId = 4,
                    GenreId = 4,
                    PublishedDate = new DateTime(1813, 1, 28),
                    Description = "A romantic novel of manners",
                    Price = 12.99m,
                    ISBN = "9780143039563",
                    StockQuantity = 60
                },
                new BookInfo 
                { 
                    Title = "A Game of Thrones",
                    AuthorId = 5,
                    GenreId = 1,
                    PublishedDate = new DateTime(1996, 8, 6),
                    Description = "Epic fantasy with intricate plotlines and political intrigue",
                    Price = 24.99m,
                    ISBN = "9780553103540",
                    StockQuantity = 40
                },
                new BookInfo 
                { 
                    Title = "The Hobbit",
                    AuthorId = 1,
                    GenreId = 1,
                    PublishedDate = new DateTime(1937, 9, 21),
                    Description = "A fantasy adventure about Bilbo Baggins and a magical journey",
                    Price = 16.99m,
                    ISBN = "9780547928227",
                    StockQuantity = 55
                },
                new BookInfo 
                { 
                    Title = "The Hound of the Baskervilles",
                    AuthorId = 6,
                    GenreId = 3,
                    PublishedDate = new DateTime(1901, 1, 1),
                    Description = "A Sherlock Holmes mystery featuring a legendary hound",
                    Price = 13.99m,
                    ISBN = "9780486282114",
                    StockQuantity = 42
                },
                new BookInfo 
                { 
                    Title = "The Shining",
                    AuthorId = 7,
                    GenreId = 5,
                    PublishedDate = new DateTime(1977, 1, 28),
                    Description = "A psychological horror thriller set in an isolated hotel",
                    Price = 17.99m,
                    ISBN = "9780385333312",
                    StockQuantity = 38
                },
                new BookInfo 
                { 
                    Title = "I, Robot",
                    AuthorId = 2,
                    GenreId = 2,
                    PublishedDate = new DateTime(1950, 12, 2),
                    Description = "A collection of short stories exploring artificial intelligence",
                    Price = 15.99m,
                    ISBN = "9780553382563",
                    StockQuantity = 48
                },
                new BookInfo 
                { 
                    Title = "Ubik",
                    AuthorId = 8,
                    GenreId = 2,
                    PublishedDate = new DateTime(1969, 5, 1),
                    Description = "A mind-bending science fiction novel about reality and consciousness",
                    Price = 16.99m,
                    ISBN = "9780679736691",
                    StockQuantity = 32
                },
                new BookInfo 
                { 
                    Title = "It",
                    AuthorId = 7,
                    GenreId = 5,
                    PublishedDate = new DateTime(1986, 9, 15),
                    Description = "An epic horror novel about childhood friends battling an evil entity",
                    Price = 22.99m,
                    ISBN = "9781501156717",
                    StockQuantity = 28
                },
                new BookInfo 
                { 
                    Title = "Emma",
                    AuthorId = 4,
                    GenreId = 4,
                    PublishedDate = new DateTime(1815, 12, 25),
                    Description = "A romantic comedy of manners about a young woman playing matchmaker",
                    Price = 11.99m,
                    ISBN = "9780143039570",
                    StockQuantity = 52
                }
            };
            await _context.Books.AddRangeAsync(books);
            await _context.SaveChangesAsync();

            // Seed Users
            var users = new List<UserInfo>
            {
                new UserInfo 
                { 
                    Username = "bookworm_alice",
                    Email = "alice@bookhaven.com",
                    RegisteredDate = DateTime.Now.AddMonths(-6)
                },
                new UserInfo 
                { 
                    Username = "mystery_bob",
                    Email = "bob@bookhaven.com",
                    RegisteredDate = DateTime.Now.AddMonths(-3)
                },
                new UserInfo 
                { 
                    Username = "scifi_charlie",
                    Email = "charlie@bookhaven.com",
                    RegisteredDate = DateTime.Now.AddDays(-30)
                }
            };
            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
        }
    }
}