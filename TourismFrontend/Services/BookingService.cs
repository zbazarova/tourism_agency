using System.Net.Http.Json;
using TourismFrontend.Models;
using System.Text.Json;

namespace TourismFrontend.Services
{
    public class BookingService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public BookingService(HttpClient httpClient, JsonSerializerOptions jsonOptions)
        {
            _httpClient = httpClient;
            _jsonOptions = jsonOptions;
        }

        public async Task CreateBookingAsync(int tourId, BookingModel model)
        {
            var booking = new
            {
                TourId = tourId,
                RoomTypeId = model.RoomTypeId,
                NumberOfPeople = model.NumberOfPeople,
                Comment = model.Comment
            };

            var response = await _httpClient.PostAsJsonAsync("api/bookings", booking, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }

        public async Task<List<Booking>> GetUserBookingsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Booking>>("api/bookings/user", _jsonOptions);
            
            return response ?? new List<Booking>();
        }

        public async Task<Booking?> CreateBookingAsync(BookingRequest booking)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/bookings", booking, _jsonOptions);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Booking>(_jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating booking: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/bookings/{bookingId}/cancel", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error canceling booking: {ex.Message}");
                throw;
            }
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Booking>($"api/bookings/{id}", _jsonOptions);
        }

        public async Task<List<RoomType>> GetRoomTypesAsync()
        {
            var roomTypes = await _httpClient.GetFromJsonAsync<List<RoomType>>("api/bookings/room-types");
            return roomTypes ?? new List<RoomType>();
        }

        public async Task DeleteBookingAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/bookings/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting booking: {ex.Message}");
                throw;
            }
        }

        public class Booking
        {
            public int Id { get; set; }
            public int TourId { get; set; }
            public Tour? Tour { get; set; }
            public DateTime BookingDate { get; set; }
            public DateTime DepartureDate { get; set; }
            public int Adults { get; set; }
            public int Children { get; set; }
            public int RoomTypeId { get; set; }
            public bool IncludeInsurance { get; set; }
            public bool IncludeTransfer { get; set; }
            public decimal TotalPrice { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        public class BookingRequest
        {
            public int TourId { get; set; }
            public DateTime DepartureDate { get; set; }
            public int Adults { get; set; }
            public int Children { get; set; }
            public int RoomTypeId { get; set; }
            public bool IncludeInsurance { get; set; }
            public bool IncludeTransfer { get; set; }
        }

        public class RoomType
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal PriceMultiplier { get; set; }
        }
    }
}
