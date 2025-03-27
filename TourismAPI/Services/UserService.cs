using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourismAPI.Data;
using TourismAPI.Models;
using TourismAPI.DTOs;
using Microsoft.AspNetCore.Identity;

namespace TourismAPI.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthService _authService;

        public UserService(ApplicationDbContext context, IPasswordHasher<User> passwordHasher, AuthService authService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _authService = authService;
        }

        public async Task<UserDto?> UpdateUserAsync(int userId, UserUpdateDto userDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(userDto.FirstName))
                user.FirstName = userDto.FirstName;
        
            if (!string.IsNullOrEmpty(userDto.LastName))
                user.LastName = userDto.LastName;
        
            if (!string.IsNullOrEmpty(userDto.Email))
            {

                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email && u.Id != userId);
                if (existingUser != null)
                {
                    throw new Exception("Этот email уже используется другим пользователем");
                }
        
                user.Email = userDto.Email;
            }
        
            if (!string.IsNullOrEmpty(userDto.PhoneNumber))
                user.Phone = userDto.PhoneNumber;

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                _authService.CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            try
            {
                await _context.SaveChangesAsync();
        
                return new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении пользователя: {ex.Message}");
                throw;
            }
        }
    }
} 