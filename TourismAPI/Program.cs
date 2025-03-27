using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using TourismAPI.Data;
using TourismAPI.Services;
using TourismAPI.Models;
using TourismAPI.Constants;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контекст базы данных
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ConfigureWarnings(warnings =>
        warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
});

// Добавляем сервисы
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TourService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Добавляем контроллеры
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Настраиваем аутентификацию
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"] ?? 
            throw new InvalidOperationException("JWT key is not configured");
            
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .SetIsOriginAllowed(_ => true) // Разрешаем все источники
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Настраиваем Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tourism API", Version = "v1" });
    
    // Настройка авторизации в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// В разделе настройки сервисов добавьте:
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

var app = builder.Build();

// Проверка конфигурации при старте
if (string.IsNullOrEmpty(app.Configuration["Jwt:Key"]))
{
    throw new InvalidOperationException(
        "JWT key is not configured. Please check your appsettings.json file.");
}

if (string.IsNullOrEmpty(app.Configuration["Jwt:Issuer"]))
{
    throw new InvalidOperationException(
        "JWT issuer is not configured. Please check your appsettings.json file.");
}

if (string.IsNullOrEmpty(app.Configuration["Jwt:Audience"]))
{
    throw new InvalidOperationException(
        "JWT audience is not configured. Please check your appsettings.json file.");
}

// Настраиваем HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Создаем базу данных и администратора
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
        
        dbContext.Database.EnsureCreated();
        
        // Проверяем, есть ли администратор
        if (!dbContext.Users.Any(u => u.Role == UserRoles.Admin))
        {
            // Создаем администратора
            authService.CreatePasswordHash("admin123", out byte[] passwordHash, out byte[] passwordSalt);
            
            var admin = new User
            {
                Email = "admin@example.com",
                FirstName = "Admin",
                LastName = "User",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = UserRoles.Admin,
                CreatedAt = DateTime.UtcNow
            };
            
            dbContext.Users.Add(admin);
            dbContext.SaveChanges();
            
            Console.WriteLine("Администратор создан: admin@example.com / admin123");
        }
    }
}

// Добавляем middleware для логирования запросов
app.Use(async (context, next) =>
{
    // Логируем входящий запрос
    Console.WriteLine($"Входящий запрос: {context.Request.Method} {context.Request.Path}");
    Console.WriteLine($"Заголовок Authorization: {context.Request.Headers["Authorization"]}");
    
    // Сохраняем оригинальный body stream
    var originalBodyStream = context.Response.Body;
    
    try
    {
        // Создаем новый memory stream для записи ответа
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        // Продолжаем обработку запроса
        await next.Invoke();
        
        // Логируем ответ
        Console.WriteLine($"Ответ: {context.Response.StatusCode}");
        
        // Копируем ответ в оригинальный stream
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
    }
    finally
    {
        // Восстанавливаем оригинальный body stream
        context.Response.Body = originalBodyStream;
    }
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads")),
    RequestPath = "/uploads"
});

app.MapControllers();

app.Run(); 