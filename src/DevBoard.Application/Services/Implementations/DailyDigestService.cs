// src/DevBoard.Application/Services/Implementations/DailyDigestService.cs
using DevBoard.Application.Services.Interfaces;
using DevBoard.Domain.Entities;
using DevBoard.Domain.Enums;
using DevBoard.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevBoard.Application.Services.Implementations;

public sealed class DailyDigestService : IDailyDigestService
{
    private readonly IRepository<Issue> _issues;
    private readonly ILogger<DailyDigestService> _logger;

    public DailyDigestService(IRepository<Issue> issues, ILogger<DailyDigestService> logger)
    {
        _issues = issues;
        _logger = logger;
    }

    public async Task SendAllAsync(CancellationToken ct = default)
    {
        var openByAssignee = await _issues.Query()
            .Where(i => i.AssigneeId != null && i.Status != IssueStatus.Done && i.Status != IssueStatus.Cancelled)
            .GroupBy(i => i.AssigneeId)
            .Select(g => new { AssigneeId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        foreach (var group in openByAssignee)
        {
            // TODO: replace with IEmailService.SendAsync once SMTP is wired up
            _logger.LogInformation("Digest: user {AssigneeId} has {Count} open issues", group.AssigneeId, group.Count);
        }
    }
}