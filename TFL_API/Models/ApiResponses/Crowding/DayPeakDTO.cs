namespace TFL_API.Models.ApiResponses.Crowding
{
    public record DayPeakDTO
    (
        string Day,
        decimal PeakPercentage,
        string PeakTimeBand
    );
}
