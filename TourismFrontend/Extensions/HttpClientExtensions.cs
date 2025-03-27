using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TourismFrontend.Extensions
{
    public static class HttpClientExtensions 
    {
        public static HttpClient ConfigureHttpClient(this HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            // Настраиваем глобальные параметры сериализации JSON
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
            
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
} 