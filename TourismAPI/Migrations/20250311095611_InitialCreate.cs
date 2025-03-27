using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TourismAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PriceMultiplier = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    IsHot = table.Column<bool>(type: "boolean", nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Inclusions = table.Column<List<string>>(type: "text[]", nullable: false),
                    Exclusions = table.Column<List<string>>(type: "text[]", nullable: false),
                    AvailableSeats = table.Column<int>(type: "integer", nullable: false),
                    Destination = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HotelName = table.Column<string>(type: "text", nullable: false),
                    HotelDescription = table.Column<string>(type: "text", nullable: false),
                    HotelStars = table.Column<int>(type: "integer", nullable: false),
                    RoomDescription = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    TransferInfo = table.Column<string>(type: "text", nullable: false),
                    AdditionalInfo = table.Column<string>(type: "text", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "bytea", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false, defaultValue: "User"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TourId = table.Column<int>(type: "integer", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Guests = table.Column<int>(type: "integer", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "Id", "Description", "Name", "PriceMultiplier" },
                values: new object[,]
                {
                    { 1, "Стандартный номер с двуспальной кроватью", "Стандартный", 1.0m },
                    { 2, "Улучшенный номер с видом на море", "Улучшенный", 1.3m },
                    { 3, "Люкс с отдельной гостиной", "Люкс", 1.8m },
                    { 4, "Семейный номер с двумя спальнями", "Семейный", 2.0m }
                });

            migrationBuilder.InsertData(
                table: "Tours",
                columns: new[] { "Id", "AdditionalInfo", "AvailableSeats", "Country", "CreatedAt", "DepartureDate", "Description", "Destination", "Discount", "Duration", "Exclusions", "HotelDescription", "HotelName", "HotelStars", "ImageUrl", "Inclusions", "IsHot", "Location", "Name", "Price", "Rating", "ReturnDate", "RoomDescription", "TransferInfo", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "", 20, "Турция", new DateTime(2025, 3, 11, 9, 56, 10, 941, DateTimeKind.Utc).AddTicks(3900), new DateTime(2024, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Насладитесь прекрасным отдыхом на пляжах Турции с системой \"всё включено\".", "", 0m, 7, new List<string>(), "", "", 0, "/images/tours/turkey-beach.jpg", new List<string>(), true, "", "Пляжный отдых в Турции", 45000m, 4.5, new DateTime(2024, 3, 22, 0, 0, 0, 0, DateTimeKind.Utc), "", "", new DateTime(2025, 3, 11, 9, 56, 10, 941, DateTimeKind.Utc).AddTicks(3900) },
                    { 2, "", 15, "Италия", new DateTime(2025, 3, 11, 9, 56, 10, 941, DateTimeKind.Utc).AddTicks(9100), new DateTime(2024, 3, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Посетите главные достопримечательности Италии: Рим, Флоренцию, Венецию.", "", 0m, 10, new List<string>(), "", "", 0, "/images/tours/italy-excursion.jpg", new List<string>(), false, "", "Экскурсионный тур по Италии", 85000m, 4.7999999999999998, new DateTime(2024, 4, 9, 0, 0, 0, 0, DateTimeKind.Utc), "", "", new DateTime(2025, 3, 11, 9, 56, 10, 941, DateTimeKind.Utc).AddTicks(9100) },
                    { 3, "", 10, "Швейцария", new DateTime(2025, 3, 11, 9, 56, 10, 941, DateTimeKind.Utc).AddTicks(9100), new DateTime(2024, 4, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Идеальный зимний отдых на лучших горнолыжных курортах Швейцарии.", "", 0m, 7, new List<string>(), "", "", 0, "/images/tours/swiss-ski.jpg", new List<string>(), false, "", "Горнолыжный курорт в Альпах", 120000m, 4.7000000000000002, new DateTime(2024, 5, 7, 0, 0, 0, 0, DateTimeKind.Utc), "", "", new DateTime(2025, 3, 11, 9, 56, 10, 941, DateTimeKind.Utc).AddTicks(9100) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TourId",
                table: "Bookings",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "RoomTypes");

            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
