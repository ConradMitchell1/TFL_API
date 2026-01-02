using TFL_API.Data;
using TFL_API.Interfaces;

namespace TFL_API.Services
{
    public class DatabaseSeeder
    {
        public static async Task SeedStationsAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var tflSeeder = scope.ServiceProvider.GetRequiredService<ITflSeederService>();
            if (!db.Stations.Any())
            {
                Console.WriteLine("Seeding tube stations...");

                var stations = await tflSeeder.GetAllTubeStationsAsync();
                db.Stations.AddRange(stations);
                await db.SaveChangesAsync();
                Console.WriteLine($"Seeded {stations.Count} tube stations.");
            }
            else 
            {
                Console.WriteLine("Tube stations already seeded.");
            }
        }
    }
}
