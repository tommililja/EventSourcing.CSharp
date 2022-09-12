using EventSourcing.Domain.Aggregates;
using EventSourcing.Domain.Common;
using EventSourcing.Domain.Events;
using EventSourcing.Domain.ValueObjects;
using EventSourcing.Domain.ValueObjects.Ids;
using NodaTime;
using Xunit;

namespace EventSourcing.Test.Aggregates;

public class MeetupTests
{
    private readonly UserId createdBy = TestData.DefaultUserId;
    
    [Fact]
    public void Meetup_Create_Should_Succeed()
    {
        var date = SystemTime.Now + Duration.FromDays(1);
        var name = MeetupName.Create("Rooftop Party");
        var invited = new HashSet<UserId> { UserId.Generate() };
        var attending = new HashSet<UserId> { createdBy };
        var status = MeetupStatus.Active;
        
        var meetup = new Meetup(date, name, createdBy, invited);
        
        Assert.NotNull(meetup.Id);
        Assert.Equal(date, meetup.Date);
        Assert.Equal(createdBy, meetup.CreatedBy);
        Assert.Equal(createdBy, meetup.Organiser);
        Assert.Equal(invited, meetup.Invited);
        Assert.Equal(attending, meetup.Attending);
        Assert.Equal(status, meetup.Status);
        Assert.False(meetup.IsCancelled);
        
        CustomAssert.Events(meetup, 1);
        CustomAssert.LastEvent<MeetupCreated>(meetup);
        CustomAssert.EventSequenceNumbers(meetup);
    }
    
    [Fact]
    public void Meetup_Create_With_Bad_Date_Should_Throw()
    {
        var date = SystemTime.Now;
        var name = MeetupName.Create("Rooftop Party");
        var invited = new HashSet<UserId> { UserId.Generate() };
        
        CustomAssert.Throws(() => new Meetup(date, name, createdBy, invited));
    }

    [Fact]
    public void Meetup_Restore_Should_Restore_Aggregate_State()
    {
        var meetupId = MeetupId.Generate();

        var now = SystemTime.Now;
        var name = MeetupName.Create("Rooftop Party");
        var newDate = SystemTime.Now + Duration.FromDays(7);
        
        var cancellationReason = MeetupCancelledStatus.Create("It's raining.");
        
        var events = new List<DomainEvent>
        {
            new MeetupCreated(meetupId, now, name, createdBy, new HashSet<UserId>()) { SequenceNumber = SequenceNumber.FromInt(0), Id = meetupId, IsInitial = true },
            new MeetupRescheduled(meetupId, newDate) { SequenceNumber = SequenceNumber.FromInt(1), Id = meetupId },
            new MeetupCancelled(meetupId, cancellationReason) { SequenceNumber = SequenceNumber.FromInt(2), Id = meetupId }
        };

        var eventStream = EventStream.Create(events);
        
        var meetup = new Meetup(meetupId, eventStream);
        
        Assert.Equal(newDate, meetup.Date);
        Assert.Equal(cancellationReason, meetup.Status);
        Assert.True(meetup.IsCancelled);
        
        CustomAssert.EventSequenceNumbers(meetup);
    }
    
    [Fact]
    public void Meetup_Reschedule_Should_Change_Date()
    {
        var newDate = SystemTime.Now + Duration.FromDays(7);

        var meetup = TestData.DefaultMeetup();
        
        meetup.Reschedule(newDate);
        
        Assert.Equal(newDate, meetup.Date);
        
        CustomAssert.Events(meetup, 2);
        CustomAssert.LastEvent<MeetupRescheduled>(meetup);
    }
    
    [Fact]
    public void Meetup_Rename_Should_Change_Name()
    {
        var newName = MeetupName.Create("New meetup name");

        var meetup = TestData.DefaultMeetup();
        
        meetup.Rename(newName);
        
        Assert.Equal(newName, meetup.Name);
        
        CustomAssert.Events(meetup, 2);
        CustomAssert.LastEvent<MeetupRenamed>(meetup);
    }
    
    [Fact]
    public void Meetup_Reschedule_With_Bad_Date_Should_Throw()
    {
        var now = SystemTime.Now;

        var meetup = TestData.DefaultMeetup();
        
        Assert.ThrowsAny<Exception>(() => meetup.Reschedule(now));
    }
    
    [Fact]
    public void Meetup_Change_Organiser_Should_Change_Organiser()
    {
        var newOrganiser = UserId.Generate();
        
        var meetup = TestData.DefaultMeetup();
        
        meetup.ChangeOrganiser(newOrganiser);
        
        Assert.Equal(newOrganiser, meetup.Organiser);
        
        CustomAssert.Events(meetup, 2);
        CustomAssert.LastEvent<MeetupOrganiserChanged>(meetup);
    }
    
    [Fact]
    public void Meetup_Invite_User_Should_Invite_User()
    {
        var newUser = UserId.Generate();

        var meetup = TestData.DefaultMeetup();
        
        meetup.InviteUser(newUser);

        Assert.Equal(2, meetup.Invited.Count);
        Assert.Contains(newUser, meetup.Invited);
        
        CustomAssert.Events(meetup, 2);
        CustomAssert.LastEvent<MeetupUserInvited>(meetup);
    }
    
    [Fact]
    public void Meetup_Remove_Invited_User_Should_Remove_User()
    {
        var newUser = UserId.Generate();

        var meetup = TestData.DefaultMeetup();
        
        meetup.InviteUser(newUser);
        meetup.RemoveUser(newUser);

        Assert.Equal(1, meetup.Invited.Count);
        Assert.DoesNotContain(newUser, meetup.Invited);
        
        CustomAssert.Events(meetup, 3);
        CustomAssert.LastEvent<MeetupUserRemoved>(meetup);
    }
    
    [Fact]
    public void Meetup_Remove_Attending_User_Should_Remove_User()
    {
        var newUser = UserId.Generate();
        
        var meetup = TestData.DefaultMeetup();
        
        meetup.InviteUser(newUser);
        meetup.Accept(newUser);
        meetup.RemoveUser(newUser);

        Assert.Equal(1, meetup.Invited.Count);
        Assert.Equal(1, meetup.Attending.Count);
        Assert.DoesNotContain(newUser, meetup.Attending);
        Assert.DoesNotContain(newUser, meetup.Invited);
        
        CustomAssert.Events(meetup, 4);
        CustomAssert.LastEvent<MeetupUserRemoved>(meetup);
    }
    
    [Fact]
    public void Meetup_User_Accept_Should_Accept_User()
    {
        var newUser = UserId.Generate();

        var meetup = TestData.DefaultMeetup();
        
        meetup.InviteUser(newUser);
        meetup.Accept(newUser);

        Assert.Equal(1, meetup.Invited.Count);
        Assert.Equal(2, meetup.Attending.Count);
        Assert.Contains(newUser, meetup.Attending);
        Assert.DoesNotContain(newUser, meetup.Invited);
        
        CustomAssert.Events(meetup, 3);
        CustomAssert.LastEvent<MeetupUserAccepted>(meetup);
    }
    
    [Fact]
    public void Meetup_User_Accept_Not_Invited_Should_Throw()
    {
        var notInvited = UserId.Generate();

        var meetup = TestData.DefaultMeetup();
        
        CustomAssert.Throws(() => meetup.Accept(notInvited));
    }
    
    [Fact]
    public void Meetup_User_Reject_Should_Reject_Invitation()
    {
        var newUser = UserId.Generate();
        
        var meetup = TestData.DefaultMeetup();
        
        meetup.InviteUser(newUser);
        meetup.Accept(newUser);
        meetup.Reject(newUser);

        Assert.Equal(2, meetup.Invited.Count);
        Assert.Equal(1, meetup.Attending.Count);
        Assert.Contains(newUser, meetup.Invited);
        Assert.DoesNotContain(newUser, meetup.Attending);
        
        CustomAssert.Events(meetup, 4);
        CustomAssert.LastEvent<MeetupUserRejected>(meetup);
    }
    
    [Fact]
    public void Meetup_User_Reject_Organiser_Should_Throw()
    {
        var meetup = TestData.DefaultMeetup();
        
        CustomAssert.Throws(() => meetup.Reject(createdBy));
    }

    [Fact]
    public void Meetup_Occurred_Should_Be_True()
    {
        var currentTime = SystemTime.Now - Duration.FromDays(7);
        
        SystemTime.ChangeCurrentTime(currentTime);
        
        var meetup = TestData.DefaultMeetup();
        
        SystemTime.Reset();
        
        Assert.True(meetup.HasOccurred);
    }
    
    [Fact]
    public void Meetup_Cancel_Should_Cancel()
    {
        var reason = MeetupCancelledStatus.Create("It's raining.");
        
        var meetup = TestData.DefaultMeetup();
        
        meetup.Cancel(reason);
        
        Assert.True(meetup.IsCancelled);
        Assert.Equal(reason, meetup.Status);
        
        CustomAssert.Events(meetup, 2);
        CustomAssert.LastEvent<MeetupCancelled>(meetup);
    }
    
    [Fact]
    public void Meetup_Cancel_Already_Started_Should_Throw()
    {
        var currentTime = SystemTime.Now - Duration.FromDays(7);
        var status = MeetupCancelledStatus.Create("It's raining.");
        
        SystemTime.ChangeCurrentTime(currentTime);
        
        var meetup = TestData.DefaultMeetup();
        
        SystemTime.Reset();
        
        CustomAssert.Throws(() => meetup.Cancel(status));
    }
}