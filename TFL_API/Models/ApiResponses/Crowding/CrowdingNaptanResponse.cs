namespace TFL_API.Models.ApiResponses.Crowding
{
    public class CrowdingNaptanResponse
    {
        public string Naptan { get; set; } = "";
        public List<CrowdingByDayResponse> DaysOfWeek { get; set; } = new();
    }
}
