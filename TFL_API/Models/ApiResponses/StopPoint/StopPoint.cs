namespace TFL_API.Models.ApiResponses.StopPoint
{
    public class StopPoint
    {
        public string CommonName { get; set; } = "";
        public string NaptanId { get; set; } = "";
        public string StopType { get; set; } = "";
        public List<string> Modes { get; set; } = new();
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
