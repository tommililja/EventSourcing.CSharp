using Light.GuardClauses;
using Light.GuardClauses.FrameworkExtensions;

namespace EventSourcing.Domain.Common;

public abstract class Aggregate<T> : IAggregate where T : AggregateId, new()
{
    private readonly Queue<DomainEvent> changes = new();

    protected Aggregate()
    {
        Id = new T();
    }

    private Aggregate(T id)
    {
        Id = id;
    }

    protected Aggregate(T id, EventStream stream) : this(id)
    {
        stream.ForEach(Apply);

        Version = stream.Version;
    }

    protected abstract void Apply(DomainEvent e);

    protected void Raise(DomainEvent e)
    {
        IsTerminal.MustBe(false, message:
            $"Cannot raise event on terminal aggregate: {Id}.");
            
        Apply(e);

        e.SequenceNumber = Version;
        e.Timestamp = SystemTime.Now; // Probably should not be assigned here.
        
        if (!e.IsInitial)
        {
            Version.Increment();
        }
     
        changes.Enqueue(e);
    }

    public void ResetChanges()
    {
        changes.Clear();
    }

    public T Id { get; }

    private SequenceNumber Version { get; } = SequenceNumber.Zero();

    protected bool IsTerminal { get; set; }

    public IReadOnlyCollection<DomainEvent> Changes => changes.AsReadOnlyList();
}