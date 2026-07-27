// src/DevBoard.Infrastructure/BackgroundServices/StaleIssueCloserWorker.cs
using DevBoard.Domain.Entities;
using DevBoard.Domain.Enums;
using DevBoard.Domain.Interfaces;
using DevBoard.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevBoard.Infrastructure.BackgroundServices;

public sealed class StaleIssueCloserWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<StaleIssueCloserWorker> _logger;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromHours(6));

    public StaleIssueCloserWorker(IServiceScopeFactory scopeFactory, ILogger<StaleIssueCloserWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StaleIssueCloserWorker starting");

        while (await _timer.WaitForNextTickAsync(stoppingToken))
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRepository<Issue>>();
            var cutoff = DateTime.UtcNow.AddDays(-30);

            var stale = await repo.Query()
                .WithStatus(IssueStatus.InReview)
                .Where(i => i.UpdatedAt < cutoff)
                .ToListAsync(stoppingToken);

            foreach (var issue in stale)
                issue.TransitionTo(IssueStatus.Cancelled);

            if (stale.Count > 0)
            {
                foreach (var issue in stale) repo.Update(issue);
                await repo.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Closed {Count} stale issues", stale.Count);
            }
        }
    }
}