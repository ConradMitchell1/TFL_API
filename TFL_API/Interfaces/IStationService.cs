using TFL_API.Models;

namespace TFL_API.Interfaces
{
    public interface IStationService
    {
        public Task AddStation(Station station);
        public Task<Station> GetStationByNaptanAsync(string naptan);
        public Task<IReadOnlyList<Station>> GetAllStationsAsync();
    }
}
