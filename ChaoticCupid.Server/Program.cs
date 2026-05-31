using ChaoticCupid.Server.Hubs;
using ChaoticCupid.Server.Services;

namespace ChaoticCupid.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR();

            builder.Services.AddSingleton<IPersonService, PersonService>();

            builder.Services.AddScoped<ICupidService, CupidService>();

            builder.Services.AddHostedService<CupidBackgroundWorker>();

            WebApplication app = builder.Build();

            app.MapHub<CupidHub>("/cupidHub");

            app.MapGet("/", () => "Chaotic Cupid server is running.");

            app.Run();
        }
    }
}