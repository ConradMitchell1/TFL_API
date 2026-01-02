using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TFL_API.Data;
using TFL_API.Interfaces;
using TFL_API.Services;

namespace TFL_API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var homeDir = Environment.GetEnvironmentVariable("HOME") ?? Directory.GetCurrentDirectory();
            var dbFolder = Path.Combine(homeDir, "data");
            Directory.CreateDirectory(dbFolder);

            var dbPath = Path.Combine(dbFolder, "app.db");

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite($"Data Source={dbPath}");
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IStationRepository, EFStationRepository>();
            builder.Services.AddScoped<IStationService, StationService>();  
            builder.Services.AddHttpClient<ITflSeederService, TflSeederService>();
            builder.Services.AddHttpClient<ICrowdingService, CrowdingService>();

            var app = builder.Build();

            await DatabaseSeeder.SeedStationsAsync(app.Services);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
