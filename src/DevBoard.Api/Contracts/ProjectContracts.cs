// src/DevBoard.Api/Contracts/ProjectContracts.cs
namespace DevBoard.Api.Contracts;

public sealed record CreateProjectRequest(string Name, string Slug, Guid WorkspaceId);

public sealed record ProjectResponse(Guid Id, string Name, string Slug, Guid WorkspaceId, int IssueCounter)
{
    public static ProjectResponse FromDomain(Domain.Entities.Project p) =>
        new(p.Id, p.Name, p.Slug.Value, p.WorkspaceId, p.IssueCounter);
}