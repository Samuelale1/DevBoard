namespace DevBoard.Domain.Entities;

using DevBoard.Domain.Exceptions;

public sealed class Comment : BaseEntity
{
    public string Content { get; private set; }

    public Guid IssueId { get; private set; }

    public Guid AuthorId { get; private set; }

    private Comment(
        string content,
        Guid issueId,
        Guid authorId)
    {
        Content = content;
        IssueId = issueId;
        AuthorId = authorId;
    }

    public static Comment Create(
        string content,
        Guid issueId,
        Guid authorId)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ValidationException("Comment cannot be empty.");

        return new Comment(content, issueId, authorId);
    }
}