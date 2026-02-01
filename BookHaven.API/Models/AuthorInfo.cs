namespace BookHaven.API.Models
{
    public class AuthorInfo
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public string Biography { get; set; }

        public string? CoverImage { get; set; }

        public override string ToString()
        {
            return $"{Name} (Born on {BirthDate.ToShortDateString()}): {Biography}";
        }

    }
}
