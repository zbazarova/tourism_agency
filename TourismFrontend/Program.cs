using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Http;
using TourismFrontend;
using TourismFrontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using TourismFrontend.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;
using TourismFrontend.Converters;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Создаем глобальные настройки JSON
var jsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = null,
    Converters = { new BookingStatusConverter() }
};

// Настраиваем базовый URL для API и JSON сериализацию
builder.Services.AddScoped(sp => 
{
    var client = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:1337")
    };
    
    return client;
});

// Регистрируем JsonSerializerOptions как синглтон
builder.Services.AddSingleton(jsonOptions);

// Создаем и настраиваем HttpClient с нашими настройками JSON
builder.Services.AddScoped<BookingService>(sp => 
{
    var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:1337") };
    return new BookingService(httpClient, jsonOptions);
});

// Регистрируем сервисы
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TourService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<StatisticsService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<VacationPlanService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ToastService>();

// Добавляем аутентификацию
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<ILogoutService, CustomAuthStateProvider>();

await builder.Build().RunAsync();
