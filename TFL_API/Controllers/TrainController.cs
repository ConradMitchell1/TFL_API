using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using TFL_API.Interfaces;

namespace TFL_API.Controllers
{
    [ApiController]
    [Route("api/[controller]/crowding")]
    public class TrainController : Controller
    {
        private readonly ICrowdingService _crowdingService;
        private readonly IStationService _stationService;
        public TrainController(ICrowdingService crowdingService, IStationService stationService)
        {
            _crowdingService = crowdingService;
            _stationService = stationService;
        }

        [HttpGet("naptan/{naptan}/day/{day}")]
        public async Task<IActionResult> ByDay([FromRoute] string naptan, [FromRoute] string day)
        {
            return Json(await _crowdingService.GetCrowdingByDayAsync(naptan, day));
        }
        [HttpGet("naptan/{naptan}")]
        public async Task<IActionResult> ByNaptan([FromRoute] string naptan)
        {
            return Json(await _crowdingService.GetCrowdingNaptanAsync(naptan));
        }
        [HttpGet("live")]
        public async Task<IActionResult> Live([FromQuery] string naptan)
        {
            return Json(await _crowdingService.GetCrowdingLiveAsync(naptan));
        }

        [HttpGet("tube-stations")]
        public async Task<IActionResult> GetTubeStations()
        {
            return Json(await _stationService.GetAllStationsAsync());
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchStations([FromQuery] string query)
        {
            var allStations = await _stationService.GetAllStationsAsync();
            var matchingStations = allStations
                .Where(s => s.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || s.Naptan.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return Json(matchingStations);
        }
    }
}
