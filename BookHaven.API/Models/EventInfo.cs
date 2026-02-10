using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookHaven.API.Models
{
    public class EventInfo
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [StringLength(100)]
        public string EventType { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        public int Capacity { get; set; }

        public int CurrentRegistrations { get; set; } = 0;

        [StringLength(500)]
        public string ImageUrl { get; set; }

        [Column(TypeName = "LONGTEXT")]
        public string CardImage { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation property for registrations
        public ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
    }

    public class EventRegistration
    {
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        public EventInfo Event { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;

        public bool IsAttended { get; set; } = false;
    }
}