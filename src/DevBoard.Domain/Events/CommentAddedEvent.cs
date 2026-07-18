namespace DevBoard.Domain.Events;

public sealed record CommentAddedEvent(
    Guid IssueId,
    Guid CommentId,
    Guid AuthorId
) : DomainEvent;