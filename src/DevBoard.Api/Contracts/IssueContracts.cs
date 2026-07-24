// src/DevBoard.Api/Contracts/IssueContracts.cs
using DevBoard.Domain.Enums;

namespace DevBoard.Api.Contracts;

public sealed record CreateIssueRequest(
    Guid ProjectId, string Title, string? Description, IssueType Type, int Priority);

public sealed record IssueStatusRequest(IssueStatus NewStatus);

public sealed record IssueResponse(
    Guid Id, string Title, string? Description, IssueStatus Status,
    IssueType Type, string Priority, string IssueKey, Guid ProjectId, Guid? AssigneeId)
{
    public static IssueResponse FromDomain(Domain.Entities.Issue i) =>
        new(i.Id, i.Title, i.Description, i.Status, i.Type, i.Priority.ToString(), i.IssueKey, i.ProjectId, i.AssigneeId);
}