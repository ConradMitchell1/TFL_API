using System.Text.Json;
using TFL_API.Interfaces;
using static System.Net.WebRequestMethods;

namespace TFL_API.Services
{
    public class JourneyService : IJourneyService
    {
        private readonly HttpClient _http;
        public JourneyService(HttpClient http)
        {
            _http = http;
        }
        public async Task<ItineraryResult> GetItinerary(string from, string to, string? date, string? time, string? timeIs)
        {
            var baseUrl = $"https://api.tfl.gov.uk/Journey/JourneyResults/{from}/to/{to}";
            var query = new List<string>();
            if (!string.IsNullOrWhiteSpace(date))
            {
                query.Add($"date={Uri.EscapeDataString(date)}");
            }
            if (!string.IsNullOrWhiteSpace(time))
            {
                query.Add($"time={Uri.EscapeDataString(time)}");
            }
            if (!string.IsNullOrWhiteSpace(timeIs))
            {
                query.Add($"timeIs={Uri.EscapeDataString(timeIs)}");
            }

            var url = query.Count > 0 ? $"{baseUrl}?{string.Join("&", query)}" : baseUrl;
            using var resp = await _http.GetAsync(url);
            var json = await resp.Content.ReadAsStringAsync();
            
            var result = JsonSerializer.Deserialize<ItineraryResult>(json);
            return result;
        }
    }
}
