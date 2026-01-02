using TFL_API.Interfaces;
using TFL_API.Models;
using TFL_API.Models.ApiResponses.StopPoint;

namespace TFL_API.Services
{
    public class TflSeederService : ITflSeederService
    {
        private readonly HttpClient _http;
        public TflSeederService(HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri("https://api.tfl.gov.uk/");
        }
        public async Task<List<Station>> GetAllTubeStationsAsync()
        {
            var lines = await GetTubeLinesAsync();
            var stationsById = new Dictionary<string, Station>();
            foreach (var line in lines) 
            {
                var stopPoints =  await GetStopPointsOnLineAsync(line.Id);
                foreach (var stopPoint in stopPoints)
                {
                    if(!string.Equals(stopPoint.StopType, "NaptanMetroStation", StringComparison.OrdinalIgnoreCase)) 
                    {
                        continue;
                    }
                    if(string.IsNullOrWhiteSpace(stopPoint.NaptanId))
                    {
                        continue;
                    }
                    stationsById[stopPoint.NaptanId] = new Station
                    {
                        Naptan = stopPoint.NaptanId,
                        Lat = stopPoint.Lat,
                        Lon = stopPoint.Lon,
                        Name = stopPoint.CommonName
                    };
                }
            }
            return stationsById.Values.ToList();
        }

        public async Task<List<StopPoint>> GetStopPointsOnLineAsync(string lineId)
        {
            var url = $"Line/{lineId}/StopPoints";
            var stopPoints = await _http.GetFromJsonAsync<List<StopPoint>>(url);
            return stopPoints ?? new List<StopPoint>();
        }

        public async Task<List<Line>> GetTubeLinesAsync()
        {
            var lines = await _http.GetFromJsonAsync<List<Line>>("Line/Mode/tube");
            return lines ?? new List<Line>();
        }
    }
}
