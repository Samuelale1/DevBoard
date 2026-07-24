using DevBoard.Application.Services.Interfaces;
using DevBoard.Domain.Entities;
using DevBoard.Domain.Enums;
using DevBoard.Domain.Interfaces;
using DevBoard.Infrastructure.Repositories;
using DevBoard.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Application.Services.Implementations;

public sealed class IssueService : IIssueService
{
    private readonly IRepository<Issue> _repository;

    public IssueService(IRepository<Issue> repository)
    {
        _repository = repository;
    }

    public async Task<Issue?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(id, ct);
    }

    public async Task<Issue?> GetByKeyAsync(
        string key,
        CancellationToken ct = default)
    {
        return await _repository
            .Query()
            .FirstOrDefaultAsync(i => i.IssueKey == key, ct);
    }

    public async Task<PagedList<Issue>> GetProjectIssuesAsync(
        Guid projectId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _repository
    .Query()
    .Where(i => i.ProjectId == projectId)
    .OrderByDescending(i => i.Priority.Level);

return await query.ToPagedListAsync(page, pageSize, ct);
    }

    public async Task<Issue> CreateAsync(
        Issue issue,
        CancellationToken ct = default)
    {
        await _repository.AddAsync(issue, ct);

        await _repository.SaveChangesAsync(ct);

        return issue;
    }

    public async Task ChangeStatusAsync(
        Guid issueId,
        IssueStatus status,
        CancellationToken ct = default)
    {
        var issue = await _repository.GetByIdAsync(issueId, ct);

        if (issue is null)
            throw new KeyNotFoundException("Issue not found.");

        issue.TransitionTo(status);

        _repository.Update(issue);

        await _repository.SaveChangesAsync(ct);
    }
}