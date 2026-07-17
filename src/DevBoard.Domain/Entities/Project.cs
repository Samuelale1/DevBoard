namespace DevBoard.Domain.Entities;

using DevBoard.Domain.Exceptions;
using DevBoard.Domain.ValueObjects;

public sealed class Project : BaseEntity
{
    public string Name { get; private set; }

    public Slug Slug { get; private set; }

    public Guid WorkspaceId { get; private set; }

    public int IssueCounter { get; private set; }

    private Project(
        string name,
        Slug slug,
        Guid workspaceId)
    {
        Name = name;
        Slug = slug;
        WorkspaceId = workspaceId;
        IssueCounter = 0;
    }

    public static Project Create(
        string name,
        Slug slug,
        Guid workspaceId )
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Project name is required.");

        if (workspaceId == Guid.Empty)
            throw new ValidationException("Workspace is required.");

        return new Project(
            name,
            slug,
            workspaceId);
    }

    public void IncrementIssueCounter()
    {
        IssueCounter++;
    }
}