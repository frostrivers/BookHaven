namespace BookHaven.API.Models
{
    public class UserInfo
    {
        public int Id { get; set; }

        public required string Username { get; set; }

        public required string Email { get; set; }

        public DateTime RegisteredDate { get; set; }

        public override string ToString()
        {
            return $"{Username} ({Email}), Registered on {RegisteredDate.ToShortDateString()}";
        }

    }
}
