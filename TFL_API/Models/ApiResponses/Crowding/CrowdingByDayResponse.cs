using System.Text.Json.Serialization;

namespace TFL_API.Models.ApiResponses.Crowding
{
    public sealed class CrowdingByDayResponse
    {
        [JsonPropertyName("naptan")]
        public string Naptan { get; set; } = "";

        [JsonPropertyName("dayOfWeek")]
        public string DayOfWeek { get; set; } = "";

        [JsonPropertyName("amPeakTimeBand")]
        public string AmPeakTimeBand { get; set; } = "";

        [JsonPropertyName("pmPeakTimeBand")]
        public string PmPeakTimeBand { get; set; } = "";

        [JsonPropertyName("timeBands")]
        public List<CrowdingTimeBand> TimeBands { get; set; } = new();

        [JsonPropertyName("isFound")]
        public bool IsFound { get; set; }

        [JsonPropertyName("isAlwaysQuiet")]
        public bool IsAlwaysQuiet { get; set; }

    }
}
