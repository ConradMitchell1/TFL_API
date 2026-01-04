using TFL_API.Models;
using TFL_API.Models.ApiResponses.Crowding;

namespace TFL_API.Interfaces
{
    public interface ICrowdingService
    {
        public Task<CrowdingByDayResponse?> GetCrowdingByDayAsync(string naptan, string day);
        public Task<CrowdingLiveResponse?> GetCrowdingLiveAsync(string naptan);
        public Task<List<CrowdingLiveResponse>> GetLiveCrowdingForStationsAsync(IEnumerable<string> naptans, int maxConcurrency = 8);
        public Task<CrowdingNaptanResponse?> GetCrowdingNaptanAsync(string naptan);
        public List<DayPeakDTO> ToWeekPeakSummary(CrowdingNaptanResponse resp);
        public List<DayPeakDTO> ToWeekQuietSummary(CrowdingNaptanResponse resp, TimeSpan start, TimeSpan end);
        public bool IsWithinWindow(string timeBand, TimeSpan start, TimeSpan end);




    }
}
