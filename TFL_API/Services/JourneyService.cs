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
        public async Task<ItineraryResult> GetItinerary(string from, string to)
        {
            var url = $"https://api.tfl.gov.uk/Journey/JourneyResults/{from}/to/{to}";
            using var resp = await _http.GetAsync(url);
            var json = await resp.Content.ReadAsStringAsync();
            
            var result = JsonSerializer.Deserialize<ItineraryResult>(json);
            return result;
        }
    }
}
