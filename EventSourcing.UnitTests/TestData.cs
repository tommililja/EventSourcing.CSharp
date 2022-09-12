using EventSourcing.Domain.Aggregates;
using EventSourcing.Domain.Common;
using EventSourcing.Domain.ValueObjects;
using EventSourcing.Domain.ValueObjects.Ids;
using NodaTime;

namespace EventSourcing.Test;

public static class TestData
{
    public static readonly UserId DefaultUserId = UserId.Generate();

    public static Meetup DefaultMeetup()
    {
        var date = SystemTime.Now + Duration.FromDays(1);
        var name = MeetupName.Create("Default");
        var invited = new HashSet<UserId> { UserId.Generate() };

        return new Meetup(date, name, DefaultUserId, invited);
    }
}