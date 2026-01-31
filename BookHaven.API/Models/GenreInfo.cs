namespace BookHaven.API.Models
{
    public class GenreInfo
    {
        public int Id { get; set; }
        
        public required string Name { get; set; }
        
        public string Description { get; set; }
        
        public override string ToString()
        {
            return $"{Name}: {Description}";
        }

    }
}
