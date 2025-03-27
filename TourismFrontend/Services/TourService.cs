using System.Net.Http.Json;
using TourismFrontend.Models;
using System.Linq;
using Blazored.LocalStorage;

namespace TourismFrontend.Services
{
    public class TourService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public TourService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<List<Tour>> GetToursAsync()
        {
            try
            {
                var tours = await _httpClient.GetFromJsonAsync<List<Tour>>("api/tours") ?? new List<Tour>();
                foreach (var tour in tours)
                {
                    if (string.IsNullOrWhiteSpace(tour.ImageUrl))
                    {
                        tour.ImageUrl = "/images/default-tour.jpg";
                    }
                }
                return tours;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching tours: {ex.Message}");
                return new List<Tour>();
            }
        }

        public async Task<Tour?> GetTourByIdAsync(int id)
        {
            try
            {
                var tour = await _httpClient.GetFromJsonAsync<Tour>($"api/tours/{id}");
                if (tour != null && string.IsNullOrWhiteSpace(tour.ImageUrl))
                {
                    tour.ImageUrl = "/images/default-tour.jpg";
                }
                return tour;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching tour {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<Tour> CreateTourAsync(Tour tour)
        {
            try
            {
                Console.WriteLine("Начало создания тура...");
                
                if (string.IsNullOrEmpty(tour.Destination))
                {
                    tour.Destination = tour.Country;
                }
                
                // Проверяем, что обязательные поля заполнены
                if (string.IsNullOrWhiteSpace(tour.Name))
                {
                    throw new Exception("Название тура обязательно");
                }
                
                if (string.IsNullOrWhiteSpace(tour.Country))
                {
                    throw new Exception("Страна обязательна");
                }
                
                if (string.IsNullOrWhiteSpace(tour.Description))
                {
                    throw new Exception("Описание обязательно");
                }
                
                if (tour.Price <= 0)
                {
                    throw new Exception("Цена должна быть больше 0");
                }
                
                if (tour.Duration <= 0)
                {
                    throw new Exception("Длительность должна быть больше 0");
                }
                
                if (tour.AvailableSeats <= 0)
                {
                    throw new Exception("Количество мест должно быть больше 0");
                }
                
                tour.DepartureDate = DateTime.SpecifyKind(tour.DepartureDate, DateTimeKind.Utc);
                tour.ReturnDate = DateTime.SpecifyKind(tour.ReturnDate, DateTimeKind.Utc);
                
                // Получаем токен из localStorage и устанавливаем заголовок авторизации
                var token = await _localStorage.GetItemAsync<string>("authToken");
                Console.WriteLine($"Токен авторизации: {(string.IsNullOrEmpty(token) ? "отсутствует" : "получен")}");
                
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    Console.WriteLine("Заголовок авторизации установлен");
                }
                else
                {
                    Console.WriteLine("ВНИМАНИЕ: Заголовок авторизации не установлен!");
                    throw new Exception("Необходима авторизация. Пожалуйста, войдите в систему.");
                }
                
                Console.WriteLine($"Отправка запроса на {_httpClient.BaseAddress}api/tours");
                var response = await _httpClient.PostAsJsonAsync("api/tours", tour);
                
                Console.WriteLine($"Получен ответ: {response.StatusCode}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Ошибка при создании тура: {response.StatusCode}, {errorContent}");
                    throw new Exception($"Ошибка при создании тура: {response.StatusCode}, {errorContent}");
                }
                
                var createdTour = await response.Content.ReadFromJsonAsync<Tour>();
                Console.WriteLine($"Тур успешно создан с ID: {createdTour?.Id}");
                return createdTour ?? new Tour();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение в CreateTourAsync: {ex.Message}");
                Console.WriteLine($"Стек вызовов: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<Tour> UpdateTourAsync(Tour tour)
        {
            try
            {
                // Получаем токен из localStorage и устанавливаем заголовок авторизации
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                
                var response = await _httpClient.PutAsJsonAsync($"api/tours/{tour.Id}", tour);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Ошибка при обновлении тура: {response.StatusCode}, {errorContent}");
                    throw new Exception($"Ошибка при обновлении тура: {response.StatusCode}, {errorContent}");
                }
                
                var updatedTour = await response.Content.ReadFromJsonAsync<Tour>();
                return updatedTour ?? throw new Exception("Не удалось получить обновленный тур");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating tour: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteTourAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/tours/{id}");
            return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting tour: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Tour>> GetHotToursAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Tour>>("api/tours/hot");
                return response ?? new List<Tour>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching hot tours: {ex.Message}");
                return new List<Tour>();
            }
        }

        public async Task<List<Tour>> SearchToursAsync(string? country = null, decimal? minPrice = null, decimal? maxPrice = null, DateTime? departureDate = null)
        {
            try
            {
                var queryParams = new List<string>();
                
                if (!string.IsNullOrEmpty(country))
                    queryParams.Add($"country={Uri.EscapeDataString(country)}");
                
                if (minPrice.HasValue)
                    queryParams.Add($"minPrice={minPrice.Value}");
                
                if (maxPrice.HasValue)
                    queryParams.Add($"maxPrice={maxPrice.Value}");
                
                if (departureDate.HasValue)
                    queryParams.Add($"departureDate={departureDate.Value:yyyy-MM-dd}");
                
                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                
                var response = await _httpClient.GetFromJsonAsync<List<Tour>>($"api/tours/search{queryString}");
                return response ?? new List<Tour>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching tours: {ex.Message}");
                return new List<Tour>();
            }
        }

        private string GetFullImageUrl(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
                return "/images/default-tour.jpg";

            if (relativeUrl.StartsWith("http"))
                return relativeUrl;

            // Используем базовый адрес API для относительных путей
            return $"{_httpClient.BaseAddress}{relativeUrl.TrimStart('/')}";
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
        {
            try
            {
                // Получаем токен из localStorage
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("Не авторизован");
                }

                var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(fileStream);
                content.Add(streamContent, "file", fileName);

                // Создаем новый HttpRequestMessage для установки заголовков
                var request = new HttpRequestMessage(HttpMethod.Post, "api/tours/upload-image")
                {
                    Content = content
                };
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ImageUploadResult>();
                    return result?.Url ?? string.Empty;
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Необходима авторизация. Пожалуйста, войдите в систему заново.");
                }
                
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Ошибка при загрузке файла: {error}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке изображения: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Tour>> GetPopularToursAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Tour>>("api/tours");
                var tours = response ?? new List<Tour>();
                
                // Сортируем туры: сначала горящие, затем по рейтингу
                return tours
                    .OrderByDescending(t => t.IsHot)
                    .ThenByDescending(t => t.Rating)
                    .Take(3)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching popular tours: {ex.Message}");
                return new List<Tour>();
            }
        }
    }

    public class ImageUploadResult
    {
        public string Url { get; set; } = string.Empty;
    }
}
