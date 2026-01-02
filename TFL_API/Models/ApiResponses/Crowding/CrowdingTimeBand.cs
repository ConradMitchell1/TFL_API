using System.Text.Json.Serialization;

namespace TFL_API.Models.ApiResponses.Crowding
{
    public sealed class CrowdingTimeBand
    {
        [JsonPropertyName("timeBand")]
        public string TimeBand { get; set; } = "";

        [JsonPropertyName("percentageOfBaseLine")]
        public decimal PercentageOfBaseLine { get; set; }
    }
}
