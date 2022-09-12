using EventSourcing.Domain.ValueObjects;
using Xunit;

namespace EventSourcing.Test.ValueObjects;

public class MeetupNameTests
{
    [Fact]
    public void MeetupName_Create_Should_Succeed()
    {
        const string name = "Default";
        
        var meetupName = MeetupName
            .Create(name)
            .ToString();

        Assert.Equal(name, meetupName);
    }
    
    [Fact]
    public void MeetupName_Create_Short_Should_Throw()
    {
        CustomAssert.Throws(() => MeetupName.Create("A"));
    }
    
    [Fact]
    public void MeetupName_Create_Long_Should_Throw()
    {
        CustomAssert.Throws(() => MeetupName.Create("A very very very long meetup name"));
    }
    
    [Fact]
    public void MeetupName_Should_Equal()
    {
        const string name = "Default";

        var meetupName = MeetupName.Create(name);
        var meetupName2 = MeetupName.Create(name);

        Assert.True(meetupName == meetupName2);
    }
    
    [Fact]
    public void MeetupName_Should_Not_Equal()
    {
        var meetupName = MeetupName.Create("Default");
        var meetupName2 = MeetupName.Create("Default2");

        Assert.True(meetupName != meetupName2);
    }
}