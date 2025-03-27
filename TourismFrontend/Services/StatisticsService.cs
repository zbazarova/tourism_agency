using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TourismFrontend.Models;

namespace TourismFrontend.Services
{
    public class StatisticsService
    {
        private readonly HttpClient _httpClient;

        public StatisticsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CountryStatistics>> GetCountryStatisticsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<CountryStatistics>>("api/statistics/countries");
                return response ?? new List<CountryStatistics>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching statistics: {ex.Message}");
                return new List<CountryStatistics>();
            }
        }
    }
} 