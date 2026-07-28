// src/DevBoard.Application/Services/Implementations/IssueService.cs
using DevBoard.Application.Services.Interfaces;
using DevBoard.Domain.Entities;
using DevBoard.Domain.Enums;
using DevBoard.Domain.Exceptions;
using DevBoard.Domain.Interfaces;
using DevBoard.Domain.ValueObjects;
using DevBoard.Shared.Common;
using DevBoard.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DevBoard.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;
using DevBoard.Application.Import;

namespace DevBoard.Application.Services.Implementations;

public sealed class IssueService : IIssueService
{
    
    private readonly IRepository<Issue> _issueRepository;
    private readonly IRepository<Project> _projectRepository;
    private readonly IRepository<AuditLogEntry> _auditLogRepository;

    private readonly ILogger<IssueService> _logger;

    private readonly IHubContext<BoardHub> _hub;

    public IssueService(IRepository<Issue> issueRepository, 
        IRepository<Project> projectRepository, 
        IRepository<AuditLogEntry> auditLogRepository,
        ILogger<IssueService> logger, 
        IHubContext<BoardHub> hub
        )
    {
        _issueRepository = issueRepository;
        _projectRepository = projectRepository;
        _auditLogRepository = auditLogRepository;
        _logger = logger;
        _hub =hub;
    }

    public async IAsyncEnumerable<AuditLogEntry> StreamAuditLogAsync(
    Guid issueId,
    [EnumeratorCancellation] CancellationToken ct = default)
        {
            var cursor = DateTime.MinValue;

            while (true)
            {
                var batch = await _auditLogRepository.Query()
                    .Where(a => a.IssueId == issueId && a.CreatedAt > cursor)
                    .OrderBy(a => a.CreatedAt)
                    .Take(50)
                    .ToListAsync(ct);

                if (batch.Count == 0) yield break;

                foreach (var entry in batch)
                    yield return entry;

                cursor = batch[^1].CreatedAt;   
            }
        }

    public Task<Issue?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _issueRepository.GetByIdAsync(id, ct);

    public Task<Issue?> GetByKeyAsync(string key, CancellationToken ct = default)
        => _issueRepository.Query().FirstOrDefaultAsync(i => i.IssueKey == key, ct);

    public Task<PagedList<Issue>> GetProjectIssuesAsync(Guid projectId, int page, int pageSize, CancellationToken ct = default)
        => _issueRepository.Query()
            .Where(i => i.ProjectId == projectId)
            .OrderByDescending(i => i.Priority.Level)
            .ToPagedListAsync(page, pageSize, ct);   // Refactor : this extension lives in DevBoard.Infrastructure.Extensions

    public async Task<Issue> CreateAsync(
        Guid projectId, string title, string? description,
        IssueType type, IssuePriority priority, CancellationToken ct = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, ct)
            ?? throw new NotFoundException($"Project {projectId} not found.");

        project.IncrementIssueCounter();
        var issueKey = $"{project.Slug.Value.ToUpperInvariant()}-{project.IssueCounter}";

        var issue = Issue.Create(title, description, type, priority, issueKey, projectId);

        _projectRepository.Update(project);
        await _issueRepository.AddAsync(issue, ct);
        await _issueRepository.SaveChangesAsync(ct);
        _logger.LogInformation("Created issue {IssueKey} in project {ProjectId}",
            issueKey,
            projectId
        );

        return issue;
    }

    public async Task ChangeStatusAsync(Guid issueId, IssueStatus status, CancellationToken ct = default)
    {
        var issue = await _issueRepository.GetByIdAsync(issueId, ct)
            ?? throw new NotFoundException($"Issue {issueId} not found.");

        _logger.LogInformation("Issue {IssueId} transitioning {From} -> {To}",issueId,issue.Status,status);

        var previousStatus = issue.Status;          
        issue.TransitionTo(status);

        var entry = AuditLogEntry.Create(issue.Id, "status_changed", $"{issue.Status} -> {status}");
        await _auditLogRepository.AddAsync(entry, ct);

        _issueRepository.Update(issue);
        await _issueRepository.SaveChangesAsync(ct); 

         await _hub.Clients
            .Group($"project:{issue.ProjectId}")
            .SendAsync("IssueUpdated", new { issue.Id, issue.Status }, ct);

    }

    public async Task<int> ImportCsvAsync(Guid projectId, Stream csvStream, CancellationToken ct = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, ct)
            ?? throw new NotFoundException($"Project {projectId} not found.");

        using var reader = new StreamReader(csvStream);
        var imported = 0;
        string? line;

        while ((line = await reader.ReadLineAsync(ct)) is not null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var row = CsvIssueParser.ParseLine(line);
            project.IncrementIssueCounter();
            var issueKey = $"{project.Slug.Value.ToUpperInvariant()}-{project.IssueCounter}";

            var issue = Issue.Create(row.Title, null, row.Type, IssuePriority.From(row.Priority), issueKey, projectId);
            await _issueRepository.AddAsync(issue, ct);
            imported++;
        }

        _projectRepository.Update(project);
        await _issueRepository.SaveChangesAsync(ct);
        return imported;
    }

}