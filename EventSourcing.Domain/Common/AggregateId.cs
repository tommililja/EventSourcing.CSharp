using Light.GuardClauses;

namespace EventSourcing.Domain.Common;

public abstract class AggregateId : ValueObject
{
    private readonly Guid id;
        
    protected AggregateId()
    {
        id = Guid.NewGuid();
    }

    protected AggregateId(Guid id)
    {
        id.MustNotBeEmpty(message:
            $"Invalid guid: {id}.");

        this.id = id;
    }

    protected override IEnumerable<object> Values
    {
        get { yield return id; }
    }

    public Guid ToGuid() => id;

    public override string ToString() => id.ToString();
}