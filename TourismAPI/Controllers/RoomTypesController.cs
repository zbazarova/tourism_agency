using Microsoft.AspNetCore.Mvc;
using TourismAPI.Data;
using TourismAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace TourismAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoomTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomType>>> GetRoomTypes()
        {
            return await _context.RoomTypes.ToListAsync();
        }
    }
} 