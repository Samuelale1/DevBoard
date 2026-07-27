

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevBoard.Infrastructure.BackgroundServices;

// Local fallback interface to avoid build errors when the real
// IDailyDigestService type isn't available to this project.
internal interface IDailyDigestService
{
    Task SendAllAsync(CancellationToken cancellationToken);
}

public sealed class DailyDigestWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DailyDigestWorker> _logger;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromHours(24));

    public DailyDigestWorker(IServiceScopeFactory scopeFactory, ILogger<DailyDigestWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DailyDigestWorker starting");

        while (await _timer.WaitForNextTickAsync(stoppingToken))
        {
            // CRITICAL: BackgroundService is a singleton, DbContext is scoped —
            // never inject IRepository<T> or AppDbContext directly into this class.
            await using var scope = _scopeFactory.CreateAsyncScope();
            var digestService = scope.ServiceProvider.GetRequiredService<IDailyDigestService>();
            await digestService.SendAllAsync(stoppingToken);
        }
    }
}