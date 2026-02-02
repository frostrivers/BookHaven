using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookHaven.API.Data;
using BookHaven.API.Models;

namespace BookHaven.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriberController : ControllerBase
    {
        private readonly BookHavenDbContext _context;

        public SubscriberController(BookHavenDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Subscribe to newsletter
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Subscribe([FromBody] SubscriberDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if subscriber already exists
            var existingSubscriber = await _context.Subscribers
                .FirstOrDefaultAsync(s => s.Email == dto.Email);

            if (existingSubscriber != null)
            {
                if (existingSubscriber.IsActive)
                {
                    return BadRequest(new { message = "This email is already subscribed." });
                }
                else
                {
                    // Reactivate inactive subscription
                    existingSubscriber.IsActive = true;
                    existingSubscriber.SubscribedDate = DateTime.UtcNow;
                    _context.Subscribers.Update(existingSubscriber);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Successfully reactivated subscription!" });
                }
            }

            var subscriber = new SubscriberInfo
            {
                Email = dto.Email,
                Name = dto.Name,
                SubscribedDate = DateTime.UtcNow,
                IsActive = true
            };

            _context.Subscribers.Add(subscriber);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Subscribe), new { id = subscriber.Id }, 
                new { message = "Successfully subscribed to our newsletter!" });
        }

        /// <summary>
        /// Unsubscribe from newsletter
        /// </summary>
        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe([FromBody] UnsubscribeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(new { message = "Email is required." });
            }

            var subscriber = await _context.Subscribers
                .FirstOrDefaultAsync(s => s.Email == dto.Email);

            if (subscriber == null)
            {
                return NotFound(new { message = "Subscriber not found." });
            }

            subscriber.IsActive = false;
            _context.Subscribers.Update(subscriber);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully unsubscribed from newsletter." });
        }

        /// <summary>
        /// Get subscriber count
        /// </summary>
        [HttpGet("count")]
        public async Task<IActionResult> GetSubscriberCount()
        {
            var count = await _context.Subscribers
                .Where(s => s.IsActive)
                .CountAsync();

            return Ok(new { count });
        }
    }

    public class SubscriberDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class UnsubscribeDto
    {
        public string Email { get; set; }
    }
}