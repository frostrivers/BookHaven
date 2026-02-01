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
            if (_context.Users.Any() || _context.Authors.Any() || _context.ItemTypes.Any() || _context.SellItems.Any())
            {
                return;
            }

            // Seed ItemTypes
            var genres = new List<ItemTypeInfo>
            {
                new ItemTypeInfo { Name = "Books", Description = "Awesome Books" },
                new ItemTypeInfo { Name = "Magazines", Description = "Magazines for everyone" },
                new ItemTypeInfo { Name = "Products", Description = "Intriguing puzzles and secrets" }
            };
            await _context.ItemTypes.AddRangeAsync(genres);
            await _context.SaveChangesAsync();

            // Seed Authors
            var authors = new List<AuthorInfo>
            {
                new AuthorInfo
                {
                    Name = "N/A",
                    BirthDate = new DateTime(2020, 1, 3),
                    Biography = "We have no clue"
                },
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

            // Seed SellItems with actual cover images (base64 encoded)
            var books = new List<SellItemInfo>
            {
                new SellItemInfo
                {
                    Title = "Notebook",
                    AuthorId = 1,
                    ItemTypeId = 3,
                    PublishedDate = new DateTime(2020, 2, 20),
                    Description = "A notebook with text on the cover that says Book Haven Bookstore",
                    Price = 3.99m,
                    ISBN = "",
                    StockQuantity = 50,
                    CoverImage = GetImageFromFile("Client3_Notebook.png")
                },
                new SellItemInfo
                {
                    Title = "Stickers",
                    AuthorId = 1,
                    ItemTypeId = 3,
                    PublishedDate = new DateTime(2025, 2, 24),
                    Description = "A set of four Book Haven Bookstore stickers that promote reading",
                    Price = 1.99m,
                    ISBN = "",
                    StockQuantity = 50,
                    CoverImage = GetImageFromFile("Client3_Stickers.png")
                },
                new SellItemInfo
                {
                    Title = "Tote Bag",
                    AuthorId = 1,
                    ItemTypeId = 3,
                    PublishedDate = new DateTime(2025, 2, 27),
                    Description = "A canvas tote bag with black lettering that says ALL I DO IS READ READ READ",
                    Price = 2.99m,
                    ISBN = "",
                    StockQuantity = 50,
                    CoverImage = GetImageFromFile("Client3_ToteBag.png")
                },

                new SellItemInfo
                {
                    Title = "Eat",
                    AuthorId = 1,
                    ItemTypeId = 2,
                    PublishedDate = new DateTime(2021, 4, 29),
                    Description = "a magazine for foodies",
                    Price = 9.99m,
                    ISBN = "",
                    StockQuantity = 50,
                    CoverImage = GetImageFromFile("Client3_Magazine3.png")
                },

                new SellItemInfo
                {
                    Title = "Travel",
                    AuthorId = 1,
                    ItemTypeId = 2,
                    PublishedDate = new DateTime(2021, 4, 29),
                    Description = "a magazine for travelers",
                    Price = 9.99m,
                    ISBN = "",
                    StockQuantity = 50,
                    CoverImage = GetImageFromFile("Client3_Magazine2.png")
                },

                new SellItemInfo
                {
                    Title = "Ball",
                    AuthorId = 1,
                    ItemTypeId = 2,
                    PublishedDate = new DateTime(2021, 4, 29),
                    Description = "a magazine about pickleball",
                    Price = 9.99m,
                    ISBN = "",
                    StockQuantity = 50,
                    CoverImage = GetImageFromFile("Client3_Magazine1.png")
                },

                new SellItemInfo
                {
                    Title = "Sorcerer’s Shadowed Chronicles",
                    AuthorId = 1,
                    ItemTypeId = 1,
                    PublishedDate = new DateTime(2001, 9, 29),
                    Description = "a fantasy book",
                    Price = 29.99m,
                    ISBN = "",
                    StockQuantity = 50,
                    CoverImage = GetImageFromFile("Client3_Book3.png")
                },

                new SellItemInfo
                {
                    Title = "Brie Mine 4Ever",
                    AuthorId = 1,
                    ItemTypeId = 1,
                    PublishedDate = new DateTime(1954, 7, 29),
                    Description = "a book for cheese lovers",
                    Price = 29.99m,
                    ISBN = "",
                    StockQuantity = 50,
                    CoverImage = GetImageFromFile("Client3_Book1.png")
                },

                new SellItemInfo
                {
                    Title = "Glory Riders",
                    AuthorId = 1,
                    ItemTypeId = 1,
                    PublishedDate = new DateTime(2005, 7, 29),
                    Description = "a book about bikers",
                    Price = 29.99m,
                    ISBN = "",
                    StockQuantity = 50,
                    CoverImage = GetImageFromFile("Client3_Book2.png")
                },

                new SellItemInfo
                {
                    Title = "The Lord of the Rings",
                    AuthorId = 2,
                    ItemTypeId = 1,
                    PublishedDate = new DateTime(1954, 7, 29),
                    Description = "An epic fantasy adventure",
                    Price = 29.99m,
                    ISBN = "9780544003415",
                    StockQuantity = 50,
                    CoverImage = GetImageFromFile("lord-of-rings.jpg")
                },
                new SellItemInfo
                {
                    Title = "Foundation",
                    AuthorId = 3,
                    ItemTypeId = 1,
                    PublishedDate = new DateTime(1951, 6, 1),
                    Description = "A science fiction masterpiece",
                    Price = 18.99m,
                    ISBN = "9780553293357",
                    StockQuantity = 35,
                    CoverImage = GetImageFromFile("foundation.jpg")
                },
                new SellItemInfo
                {
                    Title = "Murder on the Orient Express",
                    AuthorId = 4,
                    ItemTypeId = 1,
                    PublishedDate = new DateTime(1934, 1, 1),
                    Description = "A classic murder mystery",
                    Price = 14.99m,
                    ISBN = "9780062073556",
                    StockQuantity = 45,
                    CoverImage = GetImageFromFile("murder-orient-express.jpg")
                },
                new SellItemInfo
                {
                    Title = "Pride and Prejudice",
                    AuthorId = 5,
                    ItemTypeId = 1,
                    PublishedDate = new DateTime(1813, 1, 28),
                    Description = "A romantic novel of manners",
                    Price = 12.99m,
                    ISBN = "9780143039563",
                    StockQuantity = 60,
                    CoverImage = GetImageFromFile("pride-prejudice.jpg")
                },
                new SellItemInfo
                {
                    Title = "A Game of Thrones",
                    AuthorId = 6,
                    ItemTypeId = 1,
                    PublishedDate = new DateTime(1996, 8, 6),
                    Description = "Epic fantasy with intricate plotlines and political intrigue",
                    Price = 24.99m,
                    ISBN = "9780553103540",
                    StockQuantity = 40,
                    CoverImage = GetImageFromFile("game-of-thrones.jpg")
                },
                new SellItemInfo
                {
                    Title = "The Hobbit",
                    AuthorId = 2,
                    ItemTypeId = 1,
                    PublishedDate = new DateTime(1937, 9, 21),
                    Description = "A fantasy adventure about Bilbo Baggins and a magical journey",
                    Price = 16.99m,
                    ISBN = "9780547928227",
                    StockQuantity = 55,
                    CoverImage = GetImageFromFile("hobbit.jpg")
                },
                new SellItemInfo
                {
                    Title = "The Hound of the Baskervilles",
                    AuthorId = 7,
                    ItemTypeId = 1,
                    PublishedDate = new DateTime(1901, 1, 1),
                    Description = "A Sherlock Holmes mystery featuring a legendary hound",
                    Price = 13.99m,
                    ISBN = "9780486282114",
                    StockQuantity = 42,
                    CoverImage = GetImageFromFile("hound-baskervilles.jpg")
                },
                new SellItemInfo
                {
                    Title = "The Shining",
                    AuthorId = 8,
                    ItemTypeId = 1,
                    PublishedDate = new DateTime(1977, 1, 28),
                    Description = "A psychological horror thriller set in an isolated hotel",
                    Price = 17.99m,
                    ISBN = "9780385333312",
                    StockQuantity = 38,
                    CoverImage = GetImageFromFile("the-shining.jpg")
                },
                new SellItemInfo
                {
                    Title = "I, Robot",
                    AuthorId = 3,
                    ItemTypeId = 1,
                    PublishedDate = new DateTime(1950, 12, 2),
                    Description = "A collection of short stories exploring artificial intelligence",
                    Price = 15.99m,
                    ISBN = "9780553382563",
                    StockQuantity = 48,
                    CoverImage = GetImageFromFile("i-robot.jpg")
                },
                new SellItemInfo
                {
                    Title = "Ubik",
                    AuthorId = 9,
                    ItemTypeId = 1,
                    PublishedDate = new DateTime(1969, 5, 1),
                    Description = "A mind-bending science fiction novel about reality and consciousness",
                    Price = 16.99m,
                    ISBN = "9780679736691",
                    StockQuantity = 32,
                    CoverImage = GetImageFromFile("ubik.jpg")
                },
                new SellItemInfo
                {
                    Title = "It",
                    AuthorId = 8,
                    ItemTypeId = 1,
                    PublishedDate = new DateTime(1986, 9, 15),
                    Description = "An epic horror novel about childhood friends battling an evil entity",
                    Price = 22.99m,
                    ISBN = "9781501156717",
                    StockQuantity = 28,
                    CoverImage = GetImageFromFile("it.jpg")
                },
                new SellItemInfo
                {
                    Title = "Emma",
                    AuthorId = 5,
                    ItemTypeId = 1,
                    PublishedDate = new DateTime(1815, 12, 25),
                    Description = "A romantic comedy of manners about a young woman playing matchmaker",
                    Price = 11.99m,
                    ISBN = "9780143039570",
                    StockQuantity = 52,
                    CoverImage = GetImageFromFile("emma.jpg")
                }
            };
            await _context.SellItems.AddRangeAsync(books);
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

        private string GetImageFromFile(string filename)
        {
            string imagePath = Path.Combine(AppContext.BaseDirectory, "Data", "SeedImages", filename);
            if (File.Exists(imagePath))
            {
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                string base64 = Convert.ToBase64String(imageBytes);
                string extension = Path.GetExtension(filename).ToLower().TrimStart('.');
                return $"data:image/{extension};base64,{base64}";
            }
            return null;
        }
    }
}