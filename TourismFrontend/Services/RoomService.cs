using System.Net.Http.Json;
using TourismFrontend.Models;

namespace TourismFrontend.Services
{
    public class RoomService
    {
        private readonly HttpClient _httpClient;

        public RoomService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<RoomType>> GetRoomTypesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<RoomType>>("api/roomtypes");
                return response ?? new List<RoomType>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching room types: {ex.Message}");
                return new List<RoomType>();
            }
        }
    }
} 