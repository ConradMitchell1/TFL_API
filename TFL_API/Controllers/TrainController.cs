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
        [HttpGet("peak/{naptan}")]
        public async Task<IActionResult> GetWeeklyPeakSummary([FromRoute] string naptan)
        {
            var weeklyCrowding = await _crowdingService.GetCrowdingNaptanAsync(naptan);
            if (weeklyCrowding == null)
            {
                return NotFound();
            }
            return Json(_crowdingService.ToWeekPeakSummary(weeklyCrowding));
        }
        [HttpGet("quiet/{naptan}")]
        public async Task<IActionResult> GetWeeklyQuietSummary([FromRoute] string naptan, [FromQuery] string? start = "08:00", [FromQuery] string? end = "20:00")
        {
            var weeklyCrowding = await _crowdingService.GetCrowdingNaptanAsync(naptan);
            if (weeklyCrowding == null)
            {
                return NotFound();
            }
            if (!TimeSpan.TryParse(start, out var startTs))
                return BadRequest("Invalid 'start'. Use HH:mm e.g. 08:00");

            if (!TimeSpan.TryParse(end, out var endTs))
                return BadRequest("Invalid 'end'. Use HH:mm e.g. 20:00");
            return Json(_crowdingService.ToWeekQuietSummary(weeklyCrowding, startTs, endTs));

        }
        [HttpGet("live")]
        public async Task<IActionResult> Live([FromQuery] string naptan)
        {
            return Json(await _crowdingService.GetCrowdingLiveAsync(naptan));
        }
        [HttpGet("live/all")]
        public async Task<IActionResult> GetAllLiveCrowding()
        {
            var stations = await _stationService.GetAllStationsAsync();
            var naptans = stations.Select(s => s.Naptan).ToList();

            var live = await _crowdingService.GetLiveCrowdingForStationsAsync(naptans);
            return Ok(live);
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
