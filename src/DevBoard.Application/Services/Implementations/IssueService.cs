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

namespace DevBoard.Application.Services.Implementations;

public sealed class IssueService : IIssueService
{
    
    private readonly IRepository<Issue> _issueRepository;
    private readonly IRepository<Project> _projectRepository;
    private readonly ILogger<IssueService> _logger;
    public IssueService(IRepository<Issue> issueRepository, IRepository<Project> projectRepository, ILogger<IssueService> logger)
    {
        _issueRepository = issueRepository;
        _projectRepository = projectRepository;
        _logger = logger;
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
            _logger.LogInformation(
                "Issue {IssueId} transitioning {From} -> {To}",
                issueId,
                issue.Status,
                status);

        issue.TransitionTo(status);
        _issueRepository.Update(issue);
        await _issueRepository.SaveChangesAsync(ct);
    }
}