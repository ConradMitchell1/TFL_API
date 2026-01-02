using TFL_API.Models;
using TFL_API.Models.ApiResponses.StopPoint;

namespace TFL_API.Interfaces
{
    public interface ITflSeederService
    {
        public Task<List<Line>> GetTubeLinesAsync();
        public Task<List<StopPoint>> GetStopPointsOnLineAsync(string lineId);
        public Task<List<Station>> GetAllTubeStationsAsync();
    }
}
