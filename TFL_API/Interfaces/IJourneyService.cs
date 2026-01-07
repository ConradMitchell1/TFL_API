namespace TFL_API.Interfaces
{
    public interface IJourneyService
    {
        public Task<ItineraryResult> GetItinerary(string naptanID1, string naptanID2);
    }
}
