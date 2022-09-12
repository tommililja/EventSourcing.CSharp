using EventSourcing.Domain.Common;
using EventSourcing.Domain.Events;
using EventSourcing.Domain.ValueObjects;
using EventSourcing.Domain.ValueObjects.Ids;
using NodaTime;
using Xunit;

namespace EventSourcing.Test.Common;

public class EventStreamTests
{
    [Fact]
    public void EventStream_Create_Should_Succeed()
    {
        var meetupId = MeetupId.Generate();

        var created = new MeetupCreated(meetupId, Instant.MinValue, MeetupName.Create("Rooftop Party"), new UserId(), new HashSet<UserId>())
        {
            SequenceNumber = SequenceNumber.FromInt(0),
            Id = meetupId
        };

        var rescheduled = new MeetupRescheduled(meetupId, Instant.MaxValue)
        {
            SequenceNumber = SequenceNumber.FromInt(1),
            Id = meetupId
        };

        var events = new List<DomainEvent> { rescheduled, created }; // Switch order.

        var eventStream = EventStream.Create(events);

        var expectedEvents = events
            .OrderBy(e => e.SequenceNumber)
            .ToList();

        var expectedVersion = SequenceNumber.FromInt(events.Count - 1);
        var expectedEventsCount = events.Count;

        Assert.Equal(expectedEvents, eventStream);
        Assert.Equal(expectedVersion, eventStream.Version);
        Assert.Equal(expectedEventsCount, eventStream.Count);
    }
    
    [Fact]
    public void EventStream_Create_Empty()
    {
        CustomAssert.Throws(() => EventStream.Create(new List<DomainEvent>()));
    }
    
    [Fact]
    public void EventStream_Create_With_Invalid_Sequence_Should_Throw()
    {
        var meetupId = MeetupId.Generate();
        
        var created = new MeetupCreated(meetupId, Instant.MinValue, MeetupName.Create("Rooftop Party"), new UserId(), new HashSet<UserId>())
        {
            SequenceNumber = SequenceNumber.FromInt(1),
            Id = meetupId,
            IsInitial = true
        };
        
        var rescheduled = new MeetupRescheduled(meetupId, Instant.MaxValue)
        {
            SequenceNumber = SequenceNumber.FromInt(0),
            Id = meetupId
        };

        var events = new List<DomainEvent> { created, rescheduled  };

        CustomAssert.Throws(() => EventStream.Create(events));
    }
    
    [Fact]
    public void EventStream_Create_With_Missing_Versions_Should_Throw()
    {
        var meetupId = MeetupId.Generate();
        
        var created = new MeetupCreated(meetupId, Instant.MinValue, MeetupName.Create("Rooftop Party"), new UserId(), new HashSet<UserId>())
        {
            SequenceNumber = SequenceNumber.FromInt(1),
            Id = meetupId
        };

        var events = new List<DomainEvent> { created  };

        CustomAssert.Throws(() => EventStream.Create(events));
    }
    
    [Fact]
    public void EventStream_Create_With_Duplicates_Should_Throw()
    {
        var meetupId = MeetupId.Generate();
        
        var created = new MeetupCreated(meetupId, Instant.MinValue, MeetupName.Create("Rooftop Party"), new UserId(), new HashSet<UserId>())
        {
            SequenceNumber = SequenceNumber.FromInt(0),
            Id = meetupId
        };

        var rescheduled = new MeetupRescheduled(meetupId, Instant.MaxValue)
        {
            SequenceNumber = SequenceNumber.FromInt(0),
            Id = meetupId
        };

        var events = new List<DomainEvent> { created, rescheduled };

        CustomAssert.Throws(() => EventStream.Create(events));
    }

    [Fact]
    public void EventStream_Create_With_Multiple_Aggregates_Should_Throw()
    {
        var meetupId1 = MeetupId.Generate();
        var meetupId2 = MeetupId.Generate();
        
        var created = new MeetupCreated(meetupId1, Instant.MinValue, MeetupName.Create("Rooftop Party"), new UserId(), new HashSet<UserId>())
        {
            SequenceNumber = SequenceNumber.FromInt(0),
            Id = meetupId1
        };

        var rescheduled = new MeetupRescheduled(meetupId2, Instant.MaxValue)
        {
            SequenceNumber = SequenceNumber.FromInt(1),
            Id = meetupId2
        };

        var events = new List<DomainEvent> { created, rescheduled };

        CustomAssert.Throws(() => EventStream.Create(events));
    }
}