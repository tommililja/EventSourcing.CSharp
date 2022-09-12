namespace EventSourcing.Domain.Common;

public interface IAggregate
{
    public IReadOnlyCollection<DomainEvent> Changes { get; }
}