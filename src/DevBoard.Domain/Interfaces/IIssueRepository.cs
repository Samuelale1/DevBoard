using DevBoard.Domain.Entities;
using DevBoard.Shared.Common;

namespace DevBoard.Domain.Interfaces;

public interface IIssueRepository : IRepository<Issue>
{
    Task<Issue?> GetByKeyAsync(
        string key,
        CancellationToken ct = default);

    Task<PagedList<Issue>> GetProjectIssuesAsync(
        Guid projectId,
        int page,
        int pageSize,
        CancellationToken ct = default);
}