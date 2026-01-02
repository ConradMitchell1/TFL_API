using TFL_API.Models;
using TFL_API.Models.ApiResponses.Crowding;

namespace TFL_API.Interfaces
{
    public interface ICrowdingService
    {
        public Task<CrowdingByDayResponse?> GetCrowdingByDayAsync(string naptan, string day);
        public Task<CrowdingLiveResponse?> GetCrowdingLiveAsync(string naptan);
        public Task<CrowdingNaptanResponse?> GetCrowdingNaptanAsync(string naptan);




    }
}
