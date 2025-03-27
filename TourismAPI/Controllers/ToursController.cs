using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismAPI.Models;
using TourismAPI.Services;
using TourismAPI.DTOs;
using System;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace TourismAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToursController : ControllerBase
    {
        private readonly TourService _tourService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        
        public ToursController(TourService tourService, IWebHostEnvironment webHostEnvironment)
        {
            _tourService = tourService;
            _webHostEnvironment = webHostEnvironment;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tour>>> GetTours()
        {
            var tours = await _tourService.GetAllToursAsync();
            return Ok(tours);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Tour>> GetTour(int id)
        {
            var tour = await _tourService.GetTourByIdAsync(id);
            
            if (tour == null)
            {
                return NotFound();
            }
            
            return Ok(tour);
        }
        
        [HttpGet("hot")]
        public async Task<ActionResult<IEnumerable<Tour>>> GetHotTours()
        {
            var tours = await _tourService.GetHotToursAsync();
            return Ok(tours);
        }
        
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Tour>>> SearchTours(
            [FromQuery] string? country,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] DateTime? departureDate)
        {
            var tours = await _tourService.SearchToursAsync(country, minPrice, maxPrice, departureDate);
            return Ok(tours);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<TourDto>> CreateTour(TourCreateDto tourDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    Console.WriteLine($"Ошибки валидации: {string.Join(", ", errors)}");
                    return BadRequest(new { Errors = errors });
                }
                
                var tour = new Tour
                {
                    Name = tourDto.Name,
                    Description = tourDto.Description,
                    Destination = tourDto.Destination,
                    Country = tourDto.Country,
                    Price = tourDto.Price,
                    Duration = tourDto.Duration,
                    DepartureDate = DateTime.SpecifyKind(tourDto.DepartureDate, DateTimeKind.Utc),
                    ReturnDate = DateTime.SpecifyKind(tourDto.ReturnDate, DateTimeKind.Utc),
                    AvailableSeats = tourDto.AvailableSeats,
                    ImageUrl = tourDto.ImageUrl,
                    Rating = tourDto.Rating,
                    IsHot = tourDto.IsHot,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdTour = await _tourService.AddTourAsync(tour);
                
                Console.WriteLine($"Тур успешно создан с ID: {createdTour.Id}");
                
                return CreatedAtAction(nameof(GetTour), new { id = createdTour.Id }, new TourDto
                {
                    Id = createdTour.Id,
                    Name = createdTour.Name,
                    Description = createdTour.Description,
                    Destination = createdTour.Destination,
                    Country = createdTour.Country,
                    Price = createdTour.Price,
                    Duration = createdTour.Duration,
                    DepartureDate = createdTour.DepartureDate,
                    ReturnDate = createdTour.ReturnDate,
                    AvailableSeats = createdTour.AvailableSeats,
                    ImageUrl = createdTour.ImageUrl,
                    Rating = createdTour.Rating,
                    IsHot = createdTour.IsHot
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании тура: {ex.Message}");
                Console.WriteLine($"Стек вызовов: {ex.StackTrace}");
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }
        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<Tour>> UpdateTour(int id, Tour tour)
        {
            if (id != tour.Id)
            {
                return BadRequest();
            }

            var existingTour = await _tourService.GetTourByIdAsync(id);
            if (existingTour == null)
            {
                return NotFound();
            }

            existingTour.Name = tour.Name;
            existingTour.Description = tour.Description;
            existingTour.Country = tour.Country;
            existingTour.Price = tour.Price;
            existingTour.Duration = tour.Duration;
            existingTour.ImageUrl = tour.ImageUrl;
            existingTour.Rating = tour.Rating;
            existingTour.IsHot = tour.IsHot;
            existingTour.DepartureDate = tour.DepartureDate;
            existingTour.ReturnDate = tour.ReturnDate;
            existingTour.AvailableSeats = tour.AvailableSeats;
            existingTour.Destination = tour.Destination;
            existingTour.Inclusions = tour.Inclusions;
            existingTour.Exclusions = tour.Exclusions;
            existingTour.HotelName = tour.HotelName;
            existingTour.HotelDescription = tour.HotelDescription;
            existingTour.HotelStars = tour.HotelStars;
            existingTour.RoomDescription = tour.RoomDescription;
            existingTour.Location = tour.Location;
            existingTour.TransferInfo = tour.TransferInfo;
            existingTour.AdditionalInfo = tour.AdditionalInfo;
            existingTour.UpdatedAt = DateTime.UtcNow;

            try
            {
                var updatedTour = await _tourService.UpdateTourAsync(existingTour);
                return Ok(updatedTour);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteTour(int id)
        {
            var result = await _tourService.DeleteTourAsync(id);
            
            if (!result)
            {
                return NotFound();
            }
            
            return NoContent();
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не был загружен");

            try
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var fileUrl = $"{baseUrl}/uploads/{uniqueFileName}";
                return Ok(new { Url = fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при загрузке файла: {ex.Message}");
            }
        }
    }
} 