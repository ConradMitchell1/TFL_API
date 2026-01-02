using TFL_API.Interfaces;
using TFL_API.Models;

namespace TFL_API.Services
{
    public class StationService : IStationService
    {
        private readonly IStationRepository _stationRepository;
        public StationService(IStationRepository stationRepository)
        {
            _stationRepository = stationRepository;
        }
        public async Task AddStation(Station station)
        {
            await _stationRepository.AddStationAsync(station);
        }

        public async Task<IReadOnlyList<Station>> GetAllStationsAsync()
        {
            var stations = await _stationRepository.GetAllStationsAsync(); 
            return stations;
        }

        public Task<Station> GetStationByNaptanAsync(string naptan)
        {
            var station = _stationRepository.GetStationAsync(naptan);
            return station;
        }
    }
}
