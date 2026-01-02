using TFL_API.Models;

namespace TFL_API.Interfaces
{
    public interface IStationRepository
    {
        Task<Station> GetStationAsync(string naptan);
        Task<IReadOnlyList<Station>> GetAllStationsAsync();
        Task AddStationAsync(Station station);
    }
}
