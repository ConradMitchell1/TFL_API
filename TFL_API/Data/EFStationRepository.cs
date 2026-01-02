using Microsoft.EntityFrameworkCore;
using TFL_API.Interfaces;
using TFL_API.Models;

namespace TFL_API.Data
{
    public class EFStationRepository : IStationRepository
    {
        private readonly AppDbContext _db;
        public EFStationRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddStationAsync(Station station)
        {
            _db.Stations.Add(station);
            await _db.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Station>> GetAllStationsAsync()
        {
            IQueryable<Station> stations = _db.Stations;
            return await stations.ToListAsync();
        }

        public async Task<Station> GetStationAsync(string naptan)
        {
            var station = await _db.Stations.FirstOrDefaultAsync(s => s.Naptan == naptan);
            if(station != null)
            {
                return station;
            }
            return null;
        }
    }
}
