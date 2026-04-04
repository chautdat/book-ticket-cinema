using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace cinema.Services
{
    public class NowPlayingAutoSyncService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NowPlayingAutoSyncService> _logger;
        private readonly bool _enabled;
        private readonly bool _runOnStartup;
        private readonly bool _hasApiKey;
        private readonly TimeSpan _interval;
        private readonly int _maxMovies;
        private readonly int _pages;

        public NowPlayingAutoSyncService(
            IServiceProvider serviceProvider,
            ILogger<NowPlayingAutoSyncService> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            var section = configuration.GetSection("TmdbSync");

            _enabled = section.GetValue<bool?>("Enabled") ?? true;
            _runOnStartup = section.GetValue<bool?>("RunOnStartup") ?? true;

            var intervalMinutes = section.GetValue<int?>("IntervalMinutes") ?? 30;
            _interval = TimeSpan.FromMinutes(Math.Max(5, intervalMinutes));

            _maxMovies = Math.Clamp(section.GetValue<int?>("MaxMovies") ?? 20, 1, 100);
            _pages = Math.Clamp(section.GetValue<int?>("Pages") ?? 2, 1, 10);

            var apiKey = configuration["TMDB_API_KEY"] ?? configuration["Tmdb:ApiKey"];
            _hasApiKey = !string.IsNullOrWhiteSpace(apiKey);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_enabled)
            {
                _logger.LogInformation("Now Playing Auto Sync Service is disabled (TmdbSync:Enabled=false).");
                return;
            }

            if (!_hasApiKey)
            {
                _logger.LogWarning("Now Playing Auto Sync Service is disabled because TMDB key is missing.");
                return;
            }

            _logger.LogInformation(
                "Now Playing Auto Sync Service is starting. Interval={IntervalMinutes}m, MaxMovies={MaxMovies}, Pages={Pages}",
                _interval.TotalMinutes, _maxMovies, _pages);

            if (_runOnStartup)
            {
                await SyncOnce(stoppingToken);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_interval, stoppingToken);
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                await SyncOnce(stoppingToken);
            }

            _logger.LogInformation("Now Playing Auto Sync Service is stopping.");
        }

        private async Task SyncOnce(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var tmdbSyncService = scope.ServiceProvider.GetRequiredService<TmdbMovieSyncService>();
                var result = await tmdbSyncService.SyncNowPlayingAsync(_maxMovies, _pages);

                _logger.LogInformation(
                    "TMDB auto-sync finished. Created={Created}, Updated={Updated}, Disabled={Disabled}, Total={Total}",
                    result.Created, result.Updated, result.Disabled, result.Total);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // expected when shutting down
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TMDB auto-sync failed.");
            }
        }
    }
}
