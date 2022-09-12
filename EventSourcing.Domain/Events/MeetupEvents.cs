using EventSourcing.Domain.Common;
using EventSourcing.Domain.ValueObjects;
using EventSourcing.Domain.ValueObjects.Ids;
using NodaTime;

namespace EventSourcing.Domain.Events;

public record MeetupCreated(AggregateId Id, Instant Date, MeetupName Name, UserId CreatedBy, HashSet<UserId> Invited) : DomainEvent(Id, true);

public record MeetupRescheduled(AggregateId Id, Instant Date) : DomainEvent(Id);

public record MeetupRenamed(AggregateId Id, MeetupName Name) : DomainEvent(Id);

public record MeetupOrganiserChanged(AggregateId Id, UserId User) : DomainEvent(Id);

public record MeetupUserInvited(AggregateId Id, UserId User) : DomainEvent(Id);

public record MeetupUserRemoved(AggregateId Id, UserId User) : DomainEvent(Id);

public record MeetupUserAccepted(AggregateId Id, UserId User) : DomainEvent(Id);

public record MeetupUserRejected(AggregateId Id, UserId User) : DomainEvent(Id);

public record MeetupCancelled(AggregateId Id, MeetupCancelledStatus Reason) : DomainEvent(Id);