using Microsoft.EntityFrameworkCore;
using TourismAPI.Data;
using TourismAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourismAPI.Services
{
    public class TourService
    {
        private readonly ApplicationDbContext _context;
        
        public TourService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<Tour>> GetAllToursAsync()
        {
            return await _context.Tours.ToListAsync();
        }
        
        public async Task<Tour?> GetTourByIdAsync(int id)
        {
            return await _context.Tours.FindAsync(id);
        }
        
        public async Task<List<Tour>> GetHotToursAsync()
        {
            return await _context.Tours.Where(t => t.IsHot).ToListAsync();
        }
        
        public async Task<List<Tour>> SearchToursAsync(string? country, decimal? minPrice, decimal? maxPrice, DateTime? departureDate)
        {
            var query = _context.Tours.AsQueryable();
            
            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(t => t.Country.Contains(country));
            }
            
            if (minPrice.HasValue)
            {
                query = query.Where(t => t.Price >= minPrice.Value);
            }
            
            if (maxPrice.HasValue)
            {
                query = query.Where(t => t.Price <= maxPrice.Value);
            }
            
            if (departureDate.HasValue)
            {
                var date = departureDate.Value.Date;
                query = query.Where(t => t.DepartureDate.Date == date);
            }
            
            return await query.ToListAsync();
        }
        
        public async Task<Tour> AddTourAsync(Tour tour)
        {
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();
            return tour;
        }
        
        public async Task<Tour> UpdateTourAsync(Tour tour)
        {
            _context.Entry(tour).State = EntityState.Modified;
            
            try
            {
                // Автоматическая скидка для горящих туров
                if (tour.IsHot)
                {
                    // Рассчитываем скидку на основе оставшихся дней до отправления
                    var daysUntilDeparture = (tour.DepartureDate - DateTime.UtcNow).Days;
                    if (daysUntilDeparture <= 7)
                    {
                        tour.Discount = 30; // 30% скидка если осталось меньше недели
                    }
                    else if (daysUntilDeparture <= 14)
                    {
                        tour.Discount = 20; // 20% скидка если осталось меньше двух недель
                    }
                    else
                    {
                        tour.Discount = 10; // 10% скидка для остальных горящих туров
                    }
                }
                else
                {
                    tour.Discount = 0;
                }
                
                await _context.SaveChangesAsync();
                return tour;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TourExistsAsync(tour.Id))
                {
                    throw new Exception("Tour not found");
                }
                throw;
            }
        }
        
        public async Task<bool> DeleteTourAsync(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
            {
                return false;
            }
            
            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();
            return true;
        }
        
        private async Task<bool> TourExistsAsync(int id)
        {
            return await _context.Tours.AnyAsync(e => e.Id == id);
        }
        
        public async Task<List<Tour>> GetAllToursWithBookingsAsync()
        {
            return await _context.Tours
                .Include(t => t.Bookings)
                .ToListAsync();
        }
    }
} 