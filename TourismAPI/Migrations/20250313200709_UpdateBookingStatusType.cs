using Microsoft.EntityFrameworkCore.Migrations;

namespace TourismAPI.Migrations
{
    public partial class UpdateBookingStatusType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Создаем временную колонку для хранения нового значения
            migrationBuilder.AddColumn<int>(
                name: "StatusNew",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Конвертируем существующие значения
            migrationBuilder.Sql(@"
                UPDATE ""Bookings"" 
                SET ""StatusNew"" = 
                    CASE 
                        WHEN ""Status"" = 'Confirmed' OR ""Status"" = 'Подтверждено' THEN 1
                        WHEN ""Status"" = 'Cancelled' OR ""Status"" = 'Отменено' THEN 2
                        ELSE 0
                    END;");

            // Удаляем старую колонку
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Bookings");

            // Переименовываем новую колонку
            migrationBuilder.RenameColumn(
                name: "StatusNew",
                table: "Bookings",
                newName: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Создаем временную колонку для хранения старого значения
            migrationBuilder.AddColumn<string>(
                name: "StatusOld",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");

            // Конвертируем значения обратно в строки
            migrationBuilder.Sql(@"
                UPDATE ""Bookings"" 
                SET ""StatusOld"" = 
                    CASE 
                        WHEN ""Status"" = 1 THEN 'Confirmed'
                        WHEN ""Status"" = 2 THEN 'Cancelled'
                        ELSE 'Pending'
                    END;");

            // Удаляем колонку с числовым значением
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Bookings");

            // Переименовываем старую колонку обратно
            migrationBuilder.RenameColumn(
                name: "StatusOld",
                table: "Bookings",
                newName: "Status");
        }
    }
}