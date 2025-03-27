using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TourismAPI.Data;
using TourismAPI.DTOs;
using TourismAPI.Models;
using TourismAPI.Constants;

namespace TourismAPI.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthService(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<AuthResponseDto?> Register(UserRegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower()))
            {
                return null;
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedAt = DateTime.UtcNow
            };

            // Первый зарегистрированный пользователь становится администратором
            if (!await _context.Users.AnyAsync())
            {
                user.Role = UserRoles.Admin;
            }
            else
            {
                user.Role = UserRoles.Customer;
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            string token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Role = user.Role,
                UserId = user.Id
            };
        }

        public async Task<AuthResponseDto?> Login(string email, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

                if (user == null)
                {
                    return null;
                }

                if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                {
                    return null;
                }

                string token = GenerateJwtToken(user);

                return new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при входе: {ex.Message}");
                throw;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? 
                throw new InvalidOperationException("JWT key is not configured");
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    public class UserRegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
} 