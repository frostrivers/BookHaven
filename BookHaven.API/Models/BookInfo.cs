namespace BookHaven.API.Models
{
    public class BookInfo
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public required int AuthorId { get; set; }

        public DateTime PublishedDate { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string ISBN { get; set; }

        public required int GenreId { get; set; }

        public int StockQuantity { get; set; }

        public override string ToString()
        {
            return $"{Title} by Author ID {AuthorId}, Published on {PublishedDate.ToShortDateString()}, Price: {Price:C}";
        }

    }
}
