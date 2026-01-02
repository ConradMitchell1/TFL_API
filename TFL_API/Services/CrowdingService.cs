using TFL_API.Interfaces;
using TFL_API.Models;
using TFL_API.Models.ApiResponses.Crowding;
using TFL_API.Models.ApiResponses.StopPoint;

namespace TFL_API.Services
{
    public class CrowdingService : ICrowdingService
    {
        private readonly HttpClient _http;
        public CrowdingService(HttpClient http)
        {
            _http = http;
        }

        public async Task<CrowdingByDayResponse?> GetCrowdingByDayAsync(string naptan, string day)
        {
            var url = $"https://api.tfl.gov.uk/crowding/{naptan}/{day}";

            var response = await _http.GetAsync(url);

            return await response.Content.ReadFromJsonAsync<CrowdingByDayResponse>();
        }

        public async Task<CrowdingLiveResponse?> GetCrowdingLiveAsync(string naptan)
        {
            var url = $"https://api.tfl.gov.uk/crowding/{naptan}/Live";
            var response = await _http.GetAsync(url);
            return await response.Content.ReadFromJsonAsync<CrowdingLiveResponse>();
        }

        public async Task<CrowdingNaptanResponse?> GetCrowdingNaptanAsync(string naptan)
        {
            var url = $"https://api.tfl.gov.uk/crowding/{naptan}";
            var response = await _http.GetAsync(url);
            return await response.Content.ReadFromJsonAsync<CrowdingNaptanResponse>();

        }



    }       
}
