namespace TFL_API.Models.ApiResponses.Crowding
{
    public class CrowdingLiveResponse
    {
        public bool DataAvailable { get; set; }
        public decimal PercentageOfBaseline { get; set; }
        public string TimeUTC { get; set; } = "";
        public string TimeLocal { get; set; } = "";

    }
}
