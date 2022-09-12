using EventSourcing.Domain.ValueObjects;
using Xunit;

namespace EventSourcing.Test.ValueObjects;

public class MeetupStatusTests
{
    [Fact]
    public void MeetupCancelledStatus_Create_Should_Succeed()
    {
        const string reason = "Default reason";
        
        var status = MeetupCancelledStatus
            .Create(reason)
            .ToString();

        Assert.Equal(reason, status);
    }
    
    [Fact]
    public void MeetupCancelledStatus_Create_Short_Should_Throw()
    {
        CustomAssert.Throws(() => MeetupCancelledStatus.Create("A"));
    }
    
    [Fact]
    public void MeetupCancelledStatus_Create_Long_Should_Throw()
    {
        var reason = new string('*', 101);
        
        CustomAssert.Throws(() => MeetupCancelledStatus.Create(reason));
    }
    
    [Fact]
    public void MeetupCancelledStatus_Should_Equal()
    {
        const string reason = "Default reason";

        var status = MeetupCancelledStatus.Create(reason);
        var status2 = MeetupCancelledStatus.Create(reason);

        Assert.True(status == status2);
    }
    
    [Fact]
    public void MeetupCancelledStatus_Should_Not_Equal()
    {
        var status = MeetupCancelledStatus.Create("Default reason");
        var status2 = MeetupCancelledStatus.Create("Default reason 2");

        Assert.True(status != status2);
    }
}