
namespace DevBoard.Domain.Events;

public sealed record IssueCreatedEvent(
    Guid IssueId,
    Guid ProjectId
) : DomainEvent;