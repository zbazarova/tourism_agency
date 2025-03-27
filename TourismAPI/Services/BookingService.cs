using Microsoft.EntityFrameworkCore;
using TourismAPI.Data;
using TourismAPI.DTOs;
using TourismAPI.Models;

namespace TourismAPI.Services
{
    public class BookingService
    {
        private readonly ApplicationDbContext _context;
        
        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<BookingDto>> GetUserBookingsAsync(int userId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Tour)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            var bookingDtos = bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                UserId = b.UserId,
                TourId = b.TourId,
                Tour = b.Tour != null ? new TourDto
                {
                    Id = b.Tour.Id,
                    Name = b.Tour.Name,
                    Description = b.Tour.Description,
                    DepartureDate = b.Tour.DepartureDate,
                    ReturnDate = b.Tour.ReturnDate,
                    Price = b.Tour.Price,
                    ImageUrl = b.Tour.ImageUrl
                } : null,
                BookingDate = b.BookingDate,
                DepartureDate = b.DepartureDate,
                Adults = b.Adults > 0 ? b.Adults : 1,
                Children = b.Children,
                RoomTypeId = b.RoomTypeId,
                RoomTypeName = "Стандартный",
                IncludeInsurance = b.IncludeInsurance,
                IncludeTransfer = b.IncludeTransfer,
                TotalPrice = b.TotalPrice,
                Status = b.Status.ToString(),
                NumberOfPeople = b.Guests
            }).ToList();

            return bookingDtos;
        }
        
        public async Task<BookingDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return null;

            return new BookingDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                User = new UserDto
                {
                    Id = booking.User.Id,
                    Email = booking.User.Email,
                    FirstName = booking.User.FirstName,
                    LastName = booking.User.LastName,
                    Role = booking.User.Role
                },
                TourId = booking.TourId,
                Tour = new TourDto
                {
                    Id = booking.Tour.Id,
                    Name = booking.Tour.Name,
                    Country = booking.Tour.Country,
                    Description = booking.Tour.Description,
                    Price = booking.Tour.Price,
                    Duration = booking.Tour.Duration,
                    ImageUrl = booking.Tour.ImageUrl,
                    Rating = booking.Tour.Rating,
                    IsHot = booking.Tour.IsHot,
                    DepartureDate = booking.Tour.DepartureDate,
                    ReturnDate = booking.Tour.ReturnDate,
                    AvailableSeats = booking.Tour.AvailableSeats
                },
                BookingDate = booking.BookingDate,
                Adults = booking.Adults > 0 ? booking.Adults : booking.Guests,
                Children = booking.Children,
                NumberOfPeople = booking.Guests,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status.ToString(),
                RoomTypeName = "Стандартный"
            };
        }
        
        public async Task<BookingDto> CreateBookingAsync(int userId, BookingCreateDto bookingDto)
        {
            var tour = await _context.Tours.FindAsync(bookingDto.TourId);
            if (tour == null)
                throw new Exception("Тур не найден");

            if (tour.AvailableSeats < bookingDto.Guests)
                throw new Exception("Недостаточно мест для бронирования");

            var booking = new Booking
            {
                UserId = userId,
                TourId = bookingDto.TourId,
                BookingDate = DateTime.UtcNow,
                Guests = bookingDto.Guests,
                TotalPrice = tour.Price * (bookingDto.Guests > 0 ? bookingDto.Guests : 1),
                Status = BookingStatus.Confirmed,
                Adults = bookingDto.Guests,
                Children = 0,
                DepartureDate = tour.DepartureDate,
                RoomTypeId = 1
            };

            // Уменьшаем количество доступных мест
            tour.AvailableSeats -= bookingDto.Guests;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return await GetBookingByIdAsync(booking.Id) ?? throw new Exception("Не удалось создать бронирование");
        }
        
        public async Task<bool> CancelBookingAsync(int id, int userId)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null || booking.UserId != userId)
                return false;

            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();
            return true;
        }
        
        public List<RoomType> GetRoomTypesAsync()
        {
            // Это просто заглушка, так как у нас больше нет RoomType в базе данных
            return new List<RoomType>
            {
                new RoomType { Id = 1, Name = "Стандартный", Description = "Стандартный номер", PriceMultiplier = 1.0m },
                new RoomType { Id = 2, Name = "Улучшенный", Description = "Улучшенный номер", PriceMultiplier = 1.3m },
                new RoomType { Id = 3, Name = "Люкс", Description = "Люкс номер", PriceMultiplier = 1.8m }
            };
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Tour)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return false;

            // Если бронирование было подтверждено, возвращаем места в тур
            if (booking.Status == BookingStatus.Confirmed)
            {
                booking.Tour.AvailableSeats += booking.Guests;
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 