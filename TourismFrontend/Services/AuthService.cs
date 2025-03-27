using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;
using TourismFrontend.Models;
using TourismFrontend.Constants;

namespace TourismFrontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly CustomAuthStateProvider _authStateProvider;
        
        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _authStateProvider = (CustomAuthStateProvider)authStateProvider;
        }
        
        public async Task<ApiResponse<bool>> RegisterAsync(UserRegisterDto user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", user);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<bool> { IsSuccess = true, Data = true };
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                return new ApiResponse<bool> { IsSuccess = false, Error = errorContent };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Error = ex.Message };
            }
        }
        
        public async Task<ApiResponse<bool>> LoginAsync(UserLoginDto user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", user);
                
                if (response.IsSuccessStatusCode)
                {
                    var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (authResult != null)
                    {
                        await _authStateProvider.UpdateAuthenticationState(authResult.Token, authResult.Role, authResult.UserId.ToString());
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
        
        public async Task Logout()
        {
            await _authStateProvider.Logout();
        }
        
        public async Task<string?> GetToken()
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        }
        
        public async Task<string?> GetUserRole()
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userRole");
        }
        
        public async Task<bool> IsUserInRole(string role)
        {
            var userRole = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userRole");
            return userRole == role;
        }
        
        public async Task<bool> IsUserAdmin()
        {
            return await IsUserInRole(UserRoles.Admin);
        }
        
        public async Task<bool> IsUserManager()
        {
            var userRole = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userRole");
            return userRole == UserRoles.Admin || userRole == UserRoles.Manager;
        }
    }

    public class UserRegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class UserLoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
