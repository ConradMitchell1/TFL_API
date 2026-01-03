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

        public bool IsWithinWindow(string timeBand, TimeSpan start, TimeSpan end)
        {
            var parts = timeBand.Split('-');
            if (parts.Length != 2)
            {
                return false;
            }
            var bandStart = TimeSpan.Parse(parts[0]);
            return bandStart >= start && bandStart < end;
        }

        public List<DayPeakDTO> ToWeekPeakSummary(CrowdingNaptanResponse resp)
        {
            List<DayPeakDTO> weekPeaks = new List<DayPeakDTO>();
            foreach (var day in resp.DaysOfWeek)
            {

                var best = day.TimeBands
                    .OrderByDescending(tb => tb.PercentageOfBaseLine)
                    .FirstOrDefault();
                decimal peakPct = best?.PercentageOfBaseLine != null ? best.PercentageOfBaseLine * 100m : 0;

                weekPeaks.Add(new DayPeakDTO
                (
                    day.DayOfWeek,
                    peakPct,
                    best?.TimeBand ?? "N/A"

                ));

            }
            return weekPeaks;
        }

        public List<DayPeakDTO> ToWeekQuietSummary(CrowdingNaptanResponse resp, TimeSpan start, TimeSpan end)
        {
            List<DayPeakDTO> weekQuiets = new List<DayPeakDTO>();
            foreach (var day in resp.DaysOfWeek)
            {
                var quietest = day.TimeBands
                    .Where(tb => IsWithinWindow(tb.TimeBand, start, end))
                    .OrderBy(tb => tb.PercentageOfBaseLine)
                    .FirstOrDefault();
                decimal quietPct = quietest?.PercentageOfBaseLine != null ? quietest.PercentageOfBaseLine * 100m : 0;
                weekQuiets.Add(new DayPeakDTO
                (
                    day.DayOfWeek,
                    quietPct,
                    quietest?.TimeBand ?? "N/A"
                ));

            }
            return weekQuiets;
        }
    }       
}
