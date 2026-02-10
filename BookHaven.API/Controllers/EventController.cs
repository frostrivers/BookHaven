using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookHaven.API.Data;
using BookHaven.API.Models;

namespace BookHaven.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly BookHavenDbContext _context;

        public EventController(BookHavenDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all active events with optional filtering
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllEvents(
            [FromQuery] string eventType = "",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;

            IQueryable<EventInfo> query = _context.Events
                .Where(e => e.IsActive && e.EventDate >= DateTime.UtcNow)
                .OrderBy(e => e.EventDate);

            if (!string.IsNullOrWhiteSpace(eventType))
            {
                query = query.Where(e => e.EventType.Contains(eventType));
            }

            var totalCount = await query.CountAsync();
            var events = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(e => e.Registrations)
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                pageNumber,
                pageSize,
                data = events
            });
        }

        /// <summary>
        /// Get event by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            return Ok(eventItem);
        }

        /// <summary>
        /// Get all event types for filtering
        /// </summary>
        [HttpGet("types/all")]
        public async Task<IActionResult> GetEventTypes()
        {
            var types = await _context.Events
                .Where(e => e.IsActive)
                .Select(e => e.EventType)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            return Ok(types);
        }

        /// <summary>
        /// Create new event (Admin only - implement auth as needed)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var eventItem = new EventInfo
            {
                Name = dto.Name,
                Description = dto.Description,
                EventType = dto.EventType,
                EventDate = dto.EventDate,
                Location = dto.Location,
                Capacity = dto.Capacity,
                ImageUrl = dto.ImageUrl,
                CardImage = dto.CardImage,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventById), new { id = eventItem.Id }, eventItem);
        }

        /// <summary>
        /// Update event (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventDto dto)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            eventItem.Name = dto.Name ?? eventItem.Name;
            eventItem.Description = dto.Description ?? eventItem.Description;
            eventItem.EventType = dto.EventType ?? eventItem.EventType;
            eventItem.EventDate = dto.EventDate != default ? dto.EventDate : eventItem.EventDate;
            eventItem.Location = dto.Location ?? eventItem.Location;
            eventItem.Capacity = dto.Capacity > 0 ? dto.Capacity : eventItem.Capacity;
            eventItem.ImageUrl = dto.ImageUrl ?? eventItem.ImageUrl;
            eventItem.CardImage = dto.CardImage ?? eventItem.CardImage;

            _context.Events.Update(eventItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Event updated successfully.", data = eventItem });
        }

        /// <summary>
        /// Cancel event (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            eventItem.IsActive = false;
            _context.Events.Update(eventItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Event cancelled successfully." });
        }

        /// <summary>
        /// Register user for event
        /// </summary>
        [HttpPost("{id}/register")]
        public async Task<IActionResult> RegisterForEvent(int id, [FromBody] EventRegistrationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var eventItem = await _context.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            if (!eventItem.IsActive)
            {
                return BadRequest(new { message = "This event has been cancelled." });
            }

            if (eventItem.EventDate < DateTime.UtcNow)
            {
                return BadRequest(new { message = "This event has already passed." });
            }

            // Check if already registered
            var existingRegistration = eventItem.Registrations
                .FirstOrDefault(r => r.Email == dto.Email);

            if (existingRegistration != null)
            {
                return BadRequest(new { message = "You are already registered for this event." });
            }

            // Check capacity
            if (eventItem.CurrentRegistrations >= eventItem.Capacity)
            {
                return BadRequest(new { message = "This event is at full capacity." });
            }

            var registration = new EventRegistration
            {
                EventId = id,
                Email = dto.Email,
                Name = dto.Name,
                RegisteredDate = DateTime.UtcNow,
                IsAttended = false
            };

            eventItem.CurrentRegistrations++;
            _context.EventRegistrations.Add(registration);
            _context.Events.Update(eventItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully registered for event!", data = registration });
        }

        /// <summary>
        /// Unregister from event
        /// </summary>
        [HttpPost("{id}/unregister")]
        public async Task<IActionResult> UnregisterFromEvent(int id, [FromBody] UnregisterEventDto dto)
        {
            var registration = await _context.EventRegistrations
                .FirstOrDefaultAsync(r => r.EventId == id && r.Email == dto.Email);

            if (registration == null)
            {
                return NotFound(new { message = "Registration not found." });
            }

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem != null)
            {
                eventItem.CurrentRegistrations = Math.Max(0, eventItem.CurrentRegistrations - 1);
                _context.Events.Update(eventItem);
            }

            _context.EventRegistrations.Remove(registration);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully unregistered from event." });
        }

        /// <summary>
        /// Get registrations for an event (Admin only)
        /// </summary>
        [HttpGet("{id}/registrations")]
        public async Task<IActionResult> GetEventRegistrations(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            var registrations = await _context.EventRegistrations
                .Where(r => r.EventId == id)
                .OrderBy(r => r.RegisteredDate)
                .ToListAsync();

            return Ok(registrations);
        }
    }

    // DTOs
    public class CreateEventDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string EventType { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public string ImageUrl { get; set; }
        public string CardImage { get; set; }
    }

    public class UpdateEventDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string EventType { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public string ImageUrl { get; set; }
        public string CardImage { get; set; }
    }

    public class EventRegistrationDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class UnregisterEventDto
    {
        public string Email { get; set; }
    }
}