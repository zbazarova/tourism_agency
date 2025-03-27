using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourismAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Adults",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Children",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DepartureDate",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IncludeInsurance",
                table: "Bookings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeTransfer",
                table: "Bookings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RoomTypeId",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Exclusions", "Inclusions", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 3, 13, 23, 26, 58, 571, DateTimeKind.Utc).AddTicks(6770), new List<string>(), new List<string>(), new DateTime(2025, 3, 13, 23, 26, 58, 571, DateTimeKind.Utc).AddTicks(6770) });

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Exclusions", "Inclusions", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 3, 13, 23, 26, 58, 572, DateTimeKind.Utc).AddTicks(2010), new List<string>(), new List<string>(), new DateTime(2025, 3, 13, 23, 26, 58, 572, DateTimeKind.Utc).AddTicks(2010) });

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Exclusions", "Inclusions", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 3, 13, 23, 26, 58, 572, DateTimeKind.Utc).AddTicks(2010), new List<string>(), new List<string>(), new DateTime(2025, 3, 13, 23, 26, 58, 572, DateTimeKind.Utc).AddTicks(2020) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adults",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Children",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DepartureDate",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "IncludeInsurance",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "IncludeTransfer",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RoomTypeId",
                table: "Bookings");

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Exclusions", "Inclusions", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 3, 13, 20, 7, 8, 970, DateTimeKind.Utc).AddTicks(7520), new List<string>(), new List<string>(), new DateTime(2025, 3, 13, 20, 7, 8, 970, DateTimeKind.Utc).AddTicks(7520) });

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Exclusions", "Inclusions", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 3, 13, 20, 7, 8, 971, DateTimeKind.Utc).AddTicks(2720), new List<string>(), new List<string>(), new DateTime(2025, 3, 13, 20, 7, 8, 971, DateTimeKind.Utc).AddTicks(2720) });

            migrationBuilder.UpdateData(
                table: "Tours",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Exclusions", "Inclusions", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 3, 13, 20, 7, 8, 971, DateTimeKind.Utc).AddTicks(2720), new List<string>(), new List<string>(), new DateTime(2025, 3, 13, 20, 7, 8, 971, DateTimeKind.Utc).AddTicks(2720) });
        }
    }
}
