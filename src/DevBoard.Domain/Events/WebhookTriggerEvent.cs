using DevBoard.Domain.Enums;

namespace DevBoard.Domain.Events;

public sealed record WebhookTriggeredEvent(
    Guid WebhookId,
    WebhookEvent Event,
    string Payload
) : DomainEvent;