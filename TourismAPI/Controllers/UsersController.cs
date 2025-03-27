using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourismAPI.Data;
using TourismAPI.DTOs;
using TourismAPI.Models;
using TourismAPI.Services;
using System.ComponentModel.DataAnnotations;

namespace TourismAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;
        private readonly UserService _userService;

        public UsersController(ApplicationDbContext context, AuthService authService, UserService userService)
        {
            _context = context;
            _authService = authService;
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.Role
            }).ToList();

            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole("Admin");

            if (id != currentUserId && !isAdmin)
            {
                return Forbid();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            };

            return Ok(userDto);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Users.FindAsync(userId);
            
            if (user == null)
                return NotFound();

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };

            return Ok(userDto);
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> UpdateProfile(TourismAPI.DTOs.UserUpdateDto userDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var userIdInt = int.Parse(userId);
                var updatedUser = await _userService.UpdateUserAsync(userIdInt, userDto);
                
                if (updatedUser == null)
                {
                    return NotFound("Пользователь не найден");
                }
                
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                Console.WriteLine($"Ошибка при обновлении профиля: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, ChangePasswordDto passwordDto)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (id != currentUserId)
            {
                return Forbid();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!_authService.VerifyPasswordHash(passwordDto.CurrentPassword, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Текущий пароль неверен");
            }

            _authService.CreatePasswordHash(passwordDto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UserRoleUpdateDto roleDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Проверяем, что роль валидна
                if (roleDto.Role != "Admin" && roleDto.Role != "Manager" && roleDto.Role != "Customer")
                {
                    return BadRequest("Недопустимая роль. Допустимые значения: Admin, Manager, Customer");
                }

                user.Role = roleDto.Role;
                await _context.SaveChangesAsync();
                
                return Ok(new { Message = $"Роль пользователя изменена на {roleDto.Role}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == createDto.Email))
            {
                return BadRequest("Пользователь с таким email уже существует");
            }

            _authService.CreatePasswordHash(createDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Email = createDto.Email,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = createDto.Role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Проверяем, не пытается ли админ удалить самого себя
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (id == currentUserId)
            {
                return BadRequest("Вы не можете удалить свою учетную запись");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("current")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _context.Users.FindAsync(int.Parse(userId));
                if (user == null)
                {
                    return NotFound();
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UserRoleUpdateDto
    {
        public string Role { get; set; } = string.Empty;
    }

    public class CreateUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
        
        public string Role { get; set; } = "Customer";
    }
} 