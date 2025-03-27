using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using System.Text.Json;
using Microsoft.JSInterop;

namespace TourismFrontend.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider, ILogoutService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly HttpClient _httpClient;
        private readonly AuthenticationState _anonymous;

        public CustomAuthStateProvider(IJSRuntime jsRuntime, HttpClient httpClient)
        {
            _jsRuntime = jsRuntime;
            _httpClient = httpClient;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
                Console.WriteLine($"Получен токен: {token?.Substring(0, Math.Min(10, token?.Length ?? 0))}...");
                
                if (string.IsNullOrWhiteSpace(token))
                {
                    return _anonymous;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var claims = ParseClaimsFromJwt(token);
                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);
                
                return new AuthenticationState(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в GetAuthenticationStateAsync: {ex.Message}");
                return _anonymous;
            }
        }

        public async Task UpdateAuthenticationState(string token, string role, string userId)
        {
            ClaimsPrincipal user;

            if (!string.IsNullOrEmpty(token))
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userRole", role);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userId", userId);
                
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "user"),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.NameIdentifier, userId)
                };

                var identity = new ClaimsIdentity(claims, "jwt");
                user = new ClaimsPrincipal(identity);
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userRole");
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userId");
                
                _httpClient.DefaultRequestHeaders.Authorization = null;
                user = new ClaimsPrincipal(new ClaimsIdentity());
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task Logout()
        {
            await UpdateAuthenticationState(string.Empty, string.Empty, string.Empty);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            var claims = new List<Claim>();
            
            if (keyValuePairs != null)
            {
                foreach (var kvp in keyValuePairs)
                {
                    if (kvp.Key == "role")
                    {
                        if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var role in element.EnumerateArray())
                            {
                                claims.Add(new Claim(ClaimTypes.Role, role.GetString() ?? string.Empty));
                            }
                        }
                        else
                        {
                            claims.Add(new Claim(ClaimTypes.Role, kvp.Value.ToString() ?? string.Empty));
                        }
                    }
                    else if (kvp.Key == "nameid")
                    {
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, kvp.Value.ToString() ?? string.Empty));
                    }
                    else if (kvp.Key == "email")
                    {
                        claims.Add(new Claim(ClaimTypes.Email, kvp.Value.ToString() ?? string.Empty));
                    }
                    else if (kvp.Key != "exp" && kvp.Key != "iss" && kvp.Key != "aud" && kvp.Key != "nbf" && kvp.Key != "iat")
                    {
                        claims.Add(new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty));
                    }
                }
            }
            
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }

    public interface ILogoutService
    {
        Task Logout();
    }
} 