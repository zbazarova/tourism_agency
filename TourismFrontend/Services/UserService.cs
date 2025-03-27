using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using TourismFrontend.Models;
using System.Collections.Generic;

namespace TourismFrontend.Services
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }
        public T? Data { get; set; }
    }

    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        public User? CurrentUser { get; private set; }
        public event Action<User>? OnUserChanged;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<User>> GetCurrentUserAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/users/current");
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<User>(_jsonOptions);
                    if (user != null)
                    {
                        CurrentUser = user;
                        OnUserChanged?.Invoke(user);
                        return new ApiResponse<User> { IsSuccess = true, Data = user };
                    }
                }
                return new ApiResponse<User> { IsSuccess = false, Error = "Не удалось получить данные пользователя" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<User> { IsSuccess = false, Error = ex.Message };
            }
        }

        public async Task<ApiResponse<List<UserDto>>> GetAllUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/users");
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<UserDto>>(_jsonOptions);
                    return new ApiResponse<List<UserDto>> { IsSuccess = true, Data = users ?? new List<UserDto>() };
                }
                return new ApiResponse<List<UserDto>> { IsSuccess = false, Error = "Не удалось получить список пользователей" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<UserDto>> { IsSuccess = false, Error = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UpdateUserRoleAsync(int userId, string role)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/users/{userId}/role", new { role });
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<bool> { IsSuccess = true, Data = true };
                }
                return new ApiResponse<bool> { IsSuccess = false, Error = "Не удалось обновить роль пользователя" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Error = ex.Message };
            }
        }

        public async Task<ApiResponse<UserDto>> CreateUserAsync(NewUserModel user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/users", user, _jsonOptions);
                if (response.IsSuccessStatusCode)
                {
                    var createdUser = await response.Content.ReadFromJsonAsync<UserDto>(_jsonOptions);
                    return new ApiResponse<UserDto> { IsSuccess = true, Data = createdUser };
                }
                return new ApiResponse<UserDto> { IsSuccess = false, Error = "Не удалось создать пользователя" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserDto> { IsSuccess = false, Error = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(int userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/users/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<bool> { IsSuccess = true, Data = true };
                }
                return new ApiResponse<bool> { IsSuccess = false, Error = "Не удалось удалить пользователя" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Error = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UpdateProfileAsync(User user, string? currentPassword = null, string? newPassword = null)
        {
            try
            {
                var updateData = new
                {
                    user.FirstName,
                    user.LastName,
                    PhoneNumber = user.Phone,
                    CurrentPassword = currentPassword,
                    NewPassword = newPassword
                };

                var response = await _httpClient.PutAsJsonAsync("api/users/profile", updateData, _jsonOptions);
                
                if (response.IsSuccessStatusCode)
                {
                    var updatedUser = await response.Content.ReadFromJsonAsync<User>(_jsonOptions);
                    if (updatedUser != null)
                    {
                        CurrentUser = updatedUser;
                        OnUserChanged?.Invoke(updatedUser);
                        return new ApiResponse<bool> { IsSuccess = true, Data = true };
                    }
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                return new ApiResponse<bool> { IsSuccess = false, Error = errorContent };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Error = ex.Message };
            }
        }
    }
} 