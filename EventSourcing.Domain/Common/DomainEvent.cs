using NodaTime;

namespace EventSourcing.Domain.Common;

public abstract record DomainEvent(AggregateId Id, bool IsInitial = false) // Not good
{
    public SequenceNumber SequenceNumber { get; set; } = SequenceNumber.Zero();
    
    public Instant Timestamp { get; set; }
}