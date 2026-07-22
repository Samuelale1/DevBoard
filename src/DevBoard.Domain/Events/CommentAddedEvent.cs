namespace DevBoard.Domain.Events;

public sealed record CommentAddedEvent(
    Guid IssueId,
    Guid CommentId,
    Guid AuthorId
) : DomainEvent; // This event is raised when a comment is added to an issue. It contains the IDs of the issue, comment, and author.