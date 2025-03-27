using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismAPI.Data;
using TourismAPI.DTOs;
using TourismAPI.Models;
using TourismAPI.Services;

namespace TourismAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly BookingService _bookingService;
        
        public BookingsController(ApplicationDbContext context, BookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }
        
        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetUserBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var bookings = await _bookingService.GetUserBookingsAsync(int.Parse(userId));
            
            // Убедимся, что все бронирования имеют корректные значения
            foreach (var booking in bookings)
            {
                // Установим минимальное количество людей
                if (booking.Adults <= 0 && booking.Children <= 0)
                {
                    booking.Adults = 1;
                }
            }
            
            return Ok(bookings);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBooking(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var booking = await _bookingService.GetBookingByIdAsync(id);
            
            if (booking == null)
            {
                return NotFound();
            }
            
            if (booking.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            
            return Ok(booking);
        }
        
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(BookingCreateDto bookingDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var userIdInt = int.Parse(userId);
            
            try
            {
                var booking = await _bookingService.CreateBookingAsync(userIdInt, bookingDto);
                return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var userIdInt = int.Parse(userId);
            var success = await _bookingService.CancelBookingAsync(id, userIdInt);
            
            if (!success)
            {
                return BadRequest("Не удалось отменить бронирование");
            }

            return NoContent();
        }
        
        [HttpGet("room-types")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<RoomType>> GetRoomTypes()
        {
            var roomTypes = _bookingService.GetRoomTypesAsync();
            return Ok(roomTypes);
        }
        
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null || booking.UserId != int.Parse(userId))
            {
                return NotFound();
            }

            await _bookingService.DeleteBookingAsync(id);
            return NoContent();
        }
    }
} 