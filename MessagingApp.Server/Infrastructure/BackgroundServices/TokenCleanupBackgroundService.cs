using MessagingApp.Server.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessagingApp.Server.Infrastructure.BackgroundServices
{
    public class TokenCleanupBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TokenCleanupBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(24); // Run daily

        public TokenCleanupBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<TokenCleanupBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Token Cleanup Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DoWorkAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during token cleanup");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var refreshTokenService = scope.ServiceProvider
                .GetRequiredService<IRefreshTokenService>();

            var removedCount = await refreshTokenService.CleanupExpiredTokensAsync();

            _logger.LogInformation(
                "Token cleanup completed. Removed {Count} expired/revoked tokens",
                removedCount);
        }
    }
}