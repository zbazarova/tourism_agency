using Microsoft.AspNetCore.Mvc;
using TourismAPI.Services;
using TourismAPI.DTOs;

namespace TourismAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto request)
        {
            // Преобразуем RegisterDto в UserRegisterDto
            var userRegisterDto = new UserRegisterDto
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName
            };
            
            var response = await _authService.Register(userRegisterDto);
            if (response == null)
            {
                return BadRequest("Пользователь с таким email уже существует");
            }

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto request)
        {
            var response = await _authService.Login(request.Email, request.Password);
            if (response == null)
            {
                return BadRequest("Неверный email или пароль");
            }

            return Ok(response);
        }
    }
} 