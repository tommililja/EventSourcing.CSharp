using EventSourcing.Domain.ValueObjects.Ids;
using Xunit;

namespace EventSourcing.Test.ValueObjects.Ids;

public class MeetupIdTests
{
    [Fact]
    public void MeetupId_Create_Should_Succeed()
    {
        var guid = Guid.NewGuid();

        var meetupId = MeetupId
            .FromGuid(guid)
            .ToGuid();

        Assert.Equal(meetupId, guid);
    }
    
    [Fact]
    public void MeetupId_Create_With_Empty_Guid_Should_Throw()
    {
        var guid = Guid.Empty;

        CustomAssert.Throws(() => MeetupId.FromGuid(guid));
    }
    
    [Fact]
    public void MeetupId_Create_Empty_Should_Not_Equal_Empty_Guid()
    {
        var meetupId = MeetupId
            .Generate()
            .ToGuid();
        
        Assert.NotEqual(meetupId, Guid.Empty);
    }
    
    [Fact]
    public void MeetupId_Should_Equal()
    {
        var guid = Guid.NewGuid();

        var meetupId = MeetupId.FromGuid(guid);
        var meetupId2 = MeetupId.FromGuid(guid);

        Assert.True(meetupId == meetupId2);
    }
    
    [Fact]
    public void MeetupId_Should_Not_Equal()
    {
        var meetupId = MeetupId.Generate();
        var meetupId2 = MeetupId.Generate();

        Assert.True(meetupId != meetupId2);
    }
}