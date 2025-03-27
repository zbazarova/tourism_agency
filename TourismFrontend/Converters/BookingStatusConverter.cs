using System.Text.Json;
using System.Text.Json.Serialization;
using TourismFrontend.Models;

namespace TourismFrontend.Converters
{
    public class BookingStatusConverter : JsonConverter<BookingStatus>
    {
        public override BookingStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return (BookingStatus)reader.GetInt32();
            }
            
            var value = reader.GetString();
            return value switch
            {
                "Pending" => BookingStatus.Pending,
                "Confirmed" => BookingStatus.Confirmed,
                "Cancelled" => BookingStatus.Cancelled,
                _ => BookingStatus.Pending
            };
        }

        public override void Write(Utf8JsonWriter writer, BookingStatus value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
} 