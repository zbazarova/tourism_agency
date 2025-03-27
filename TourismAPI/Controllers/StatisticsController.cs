using Microsoft.AspNetCore.Mvc;
using TourismAPI.Services;
using TourismAPI.Models;

namespace TourismAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly TourService _tourService;

        public StatisticsController(TourService tourService)
        {
            _tourService = tourService;
        }

        [HttpGet("countries")]
        public async Task<ActionResult<IEnumerable<CountryStatistics>>> GetCountryStatistics()
        {
            var tours = await _tourService.GetAllToursWithBookingsAsync();
            
            var statistics = tours
                .GroupBy(t => t.Country)
                .Select(g => new CountryStatistics
                {
                    Country = g.Key,
                    TotalTours = g.Count(),
                    AveragePrice = g.Average(t => t.FinalPrice),
                    HotTours = g.Count(t => t.IsHot),
                    TotalBookings = g.Sum(t => t.Bookings.Count),
                    AverageRating = g.Average(t => t.Rating)
                })
                .OrderByDescending(s => s.TotalTours)
                .ToList();

            return Ok(statistics);
        }
    }
} 