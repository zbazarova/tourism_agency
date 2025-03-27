using Microsoft.EntityFrameworkCore;
using TourismAPI.Models;

namespace TourismAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Tour> Tours { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasDefaultValue("User");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Tour>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Rating).HasColumnType("double precision");
                entity.Property(e => e.DepartureDate).HasColumnType("timestamp with time zone");
                entity.Property(e => e.ReturnDate).HasColumnType("timestamp with time zone");
                entity.Property(t => t.Discount).HasColumnType("decimal(18,2)");
                entity.Ignore(t => t.FinalPrice);

                entity.HasMany(t => t.Bookings)
                      .WithOne(b => b.Tour)
                      .HasForeignKey(b => b.TourId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasOne(b => b.User)
                      .WithMany()
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(b => b.Tour)
                      .WithMany(t => t.Bookings)
                      .HasForeignKey(b => b.TourId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RoomType>().HasData(
                new RoomType { Id = 1, Name = "Стандартный", Description = "Стандартный номер с двуспальной кроватью", PriceMultiplier = 1.0m },
                new RoomType { Id = 2, Name = "Улучшенный", Description = "Улучшенный номер с видом на море", PriceMultiplier = 1.3m },
                new RoomType { Id = 3, Name = "Люкс", Description = "Люкс с отдельной гостиной", PriceMultiplier = 1.8m },
                new RoomType { Id = 4, Name = "Семейный", Description = "Семейный номер с двумя спальнями", PriceMultiplier = 2.0m }
            );
            
            modelBuilder.Entity<Tour>().HasData(
                new Tour 
                { 
                    Id = 1, 
                    Name = "Пляжный отдых в Турции", 
                    Country = "Турция", 
                    Description = "Насладитесь прекрасным отдыхом на пляжах Турции с системой \"всё включено\".",
                    Price = 45000, 
                    Duration = 7,
                    ImageUrl = "/images/tours/turkey-beach.jpg",
                    Rating = 4.5,
                    IsHot = true,
                    DepartureDate = DateTime.SpecifyKind(new DateTime(2024, 3, 15), DateTimeKind.Utc),
                    ReturnDate = DateTime.SpecifyKind(new DateTime(2024, 3, 22), DateTimeKind.Utc),
                    AvailableSeats = 20
                },
                new Tour 
                { 
                    Id = 2, 
                    Name = "Экскурсионный тур по Италии", 
                    Country = "Италия", 
                    Description = "Посетите главные достопримечательности Италии: Рим, Флоренцию, Венецию.",
                    Price = 85000, 
                    Duration = 10,
                    ImageUrl = "/images/tours/italy-excursion.jpg",
                    Rating = 4.8,
                    IsHot = false,
                    DepartureDate = DateTime.SpecifyKind(new DateTime(2024, 3, 30), DateTimeKind.Utc),
                    ReturnDate = DateTime.SpecifyKind(new DateTime(2024, 4, 9), DateTimeKind.Utc),
                    AvailableSeats = 15
                },
                new Tour 
                { 
                    Id = 3, 
                    Name = "Горнолыжный курорт в Альпах", 
                    Country = "Швейцария", 
                    Description = "Идеальный зимний отдых на лучших горнолыжных курортах Швейцарии.",
                    Price = 120000, 
                    Duration = 7,
                    ImageUrl = "/images/tours/swiss-ski.jpg",
                    Rating = 4.7,
                    IsHot = false,
                    DepartureDate = DateTime.SpecifyKind(new DateTime(2024, 4, 30), DateTimeKind.Utc),
                    ReturnDate = DateTime.SpecifyKind(new DateTime(2024, 5, 7), DateTimeKind.Utc),
                    AvailableSeats = 10
                }
            );
        }
    }
} 