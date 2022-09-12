using Light.GuardClauses;
using Light.GuardClauses.FrameworkExtensions;

namespace EventSourcing.Domain.Common;

public sealed class EventStream : ReadOnlyCollection<DomainEvent>
{
    private EventStream(IEnumerable<DomainEvent> events) : base(events.OrderBy(e => e.SequenceNumber).ToList())
    {
        Items.MustNotBeNullOrEmpty(message:
            "Cannot create empty event stream.");

        Items.First().IsInitial
            .MustBe(true, message:
                "Invalid order of events.");
        
        Items.Select(x => x.SequenceNumber)
            .Except(Enumerable
                .Range(0, Items.Count)
                .Select(SequenceNumber.FromInt)
            )
            .Count()
            .MustBe(0, message:
                "Missing versions in event stream.");
        
        Items.GroupBy(x => x.SequenceNumber)
            .All(g => g.Count() == 1)
            .MustNotBe(false, message: 
                "Duplicate versions in event stream.");

        Items.Select(x => x.Id)
            .Distinct()
            .Count()
            .MustBe(1, message:
                "Multiple aggregates in event stream.");
    }

    public void ForEach(Action<DomainEvent> action) => Items.ForEach(action);

    public static EventStream Create(IEnumerable<DomainEvent> events) => new(events);

    public SequenceNumber Version => SequenceNumber.FromInt(Count - 1);
}