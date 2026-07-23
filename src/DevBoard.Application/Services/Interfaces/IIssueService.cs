using DevBoard.Domain.Entities;
using DevBoard.Domain.Enums;
using DevBoard.Shared.Common;

namespace DevBoard.Application.Services.Interfaces;
/* 
This interface defines the contract for the IssueService, which is responsible for managing issues in the application. 
It provides methods for retrieving issues by their id,
 as well as getting Issues by their issue key,
 Retrieving all issues pertaining a project,
 creating an issue , and updating issues by changing their status,
 The methods are asynchronous and support cancellation through the CancellationToken parameter.
 The Cancellation Token allows the caller to cancel the operation if needed, 
 which is useful for long-running tasks or when the user navigates away from a page.
 */
public interface IIssueService
{
    Task<Issue?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    Task<Issue?> GetByKeyAsync(
        string key,
        CancellationToken ct = default);

    Task<PagedList<Issue>> GetProjectIssuesAsync(
        Guid projectId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<Issue> CreateAsync(
        Issue issue,
        CancellationToken ct = default);

    Task ChangeStatusAsync(
        Guid issueId,
        IssueStatus status,
        CancellationToken ct = default);
}