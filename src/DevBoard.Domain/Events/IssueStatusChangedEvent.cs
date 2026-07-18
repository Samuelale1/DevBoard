
using DevBoard.Domain.Enums;

namespace DevBoard.Domain.Events;

public sealed record IssueStatusChangedEvent(
    Guid IssueId,
    IssueStatus From,
    IssueStatus To
) : DomainEvent;