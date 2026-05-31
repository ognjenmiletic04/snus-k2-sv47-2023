namespace ChaoticCupid.Server.Services
{
    public class CupidBackgroundWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CupidBackgroundWorker(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

                using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                {
                    ICupidService cupidService = scope.ServiceProvider.GetRequiredService<ICupidService>();

                    await cupidService.SendLettersToAllAsync();
                }
            }
        }
    }
}