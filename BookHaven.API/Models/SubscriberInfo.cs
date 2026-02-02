using System.ComponentModel.DataAnnotations;

namespace BookHaven.API.Models
{
    public class SubscriberInfo
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public DateTime SubscribedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}