using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TourismAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public FileController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Файл не выбран");

                // Проверка размера файла (5MB максимум)
                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest("Размер файла не должен превышать 5MB");

                // Проверка типа файла
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                if (!allowedExtensions.Contains(extension))
                    return BadRequest("Разрешены только изображения форматов: " + string.Join(", ", allowedExtensions));

                // Создаем уникальное имя файла
                var fileName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine(_environment.WebRootPath, "images", "tours", fileName);

                // Создаем директорию, если её нет
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory!);

                // Сохраняем файл
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Возвращаем URL файла
                return Ok(new { url = $"/images/tours/{fileName}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }
    }
} 