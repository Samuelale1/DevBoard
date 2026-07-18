//every other event will inherit from this class
// it is a domain event that will be raised when something happens in the domain(business logic) 
// that other parts of the system might be interested in.

namespace DevBoard.Domain.Events;
public abstract record DomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}