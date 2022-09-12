using EventSourcing.Domain.Common;
using EventSourcing.Domain.Events;
using EventSourcing.Domain.ValueObjects;
using EventSourcing.Domain.ValueObjects.Ids;
using Light.GuardClauses;
using Light.GuardClauses.FrameworkExtensions;
using NodaTime;

namespace EventSourcing.Domain.Aggregates;

public class Meetup : Aggregate<MeetupId>
{
    private readonly HashSet<UserId> invited = new();
    private readonly HashSet<UserId> attending = new();

    public Meetup(Instant date, MeetupName name, UserId createdBy, HashSet<UserId> invited)
    {
        date.MustBeGreaterThan(SystemTime.Now, message:
            "Meetup date must be in the future.");

        Raise(new MeetupCreated(Id, date, name, createdBy, invited));
    }

    public Meetup(MeetupId id, EventStream stream) : base(id, stream)
    {
        
    }

    protected override void Apply(DomainEvent e)
    {
        switch (e)
        {
            case MeetupCreated ev:
                Date = ev.Date;
                Name = ev.Name;
                CreatedBy = ev.CreatedBy;
                Organiser = ev.CreatedBy;
                invited.UnionWith(ev.Invited);
                attending.Add(ev.CreatedBy);
                break;
            
            case MeetupRescheduled ev:
                Date = ev.Date;
                break;
            
            case MeetupRenamed ev:
                Name = ev.Name;
                break;
            
            case MeetupOrganiserChanged ev:
                Organiser = ev.User;
                break;
            
            case MeetupUserInvited ev:
                invited.Add(ev.User);
                break;
        
            case MeetupUserRemoved ev:
                invited.Remove(ev.User);
                attending.Remove(ev.User);
                break;
            
            case MeetupUserAccepted ev:
                attending.Add(ev.User);
                invited.Remove(ev.User);
                break;
        
            case MeetupUserRejected ev:
                attending.Remove(ev.User);
                invited.Add(ev.User);
                break;
            
            case MeetupCancelled ev:
                Status = ev.Reason;
                IsTerminal = true;
                break;
        }
    }

    public void Reschedule(Instant date)
    {
        date.MustBeGreaterThan(SystemTime.Now, message:
            "Date has already passed.");

        Raise(new MeetupRescheduled(Id, date));
    }

    public void Rename(MeetupName name)
    {
        if (Name == name)
            return;

        Raise(new MeetupRenamed(Id, name));
    }
    
    public void ChangeOrganiser(UserId user)
    {
        if (user == Organiser)
            return;
        
        Raise(new MeetupOrganiserChanged(Id, user));
    }

    public void InviteUser(UserId user)
    {
        if (attending.Contains(user)) 
            return;

        Raise(new MeetupUserInvited(Id, user));
    }

    public void RemoveUser(UserId user)
    {
        if (!invited.Contains(user) && !attending.Contains(user)) 
            return;

        Raise(new MeetupUserRemoved(Id, user));
    }

    public void Accept(UserId user)
    {
        invited.MustContain(user, message: 
            $"User {user} is not invited.");
        
        if (attending.Contains(user)) 
            return;

        Raise(new MeetupUserAccepted(Id, user));
    }
    
    public void Reject(UserId user)
    {
        user.MustNotBe(Organiser, message:
            "The organiser must be attending.");
        
        if (!attending.Contains(user))
            return;

        Raise(new MeetupUserRejected(Id, user));
    }

    public void Cancel(MeetupCancelledStatus reason)
    {
        HasOccurred.MustBe(false, message:
            "Meetup has already occurred.");
        
        if (IsCancelled) 
            return;
        
        Raise(new MeetupCancelled(Id, reason));
    }
    
    public Instant Date { get; private set; }

    public MeetupName Name { get; private set; }
    
    public UserId CreatedBy { get; private set; }

    public UserId Organiser { get; private set; }

    public IReadOnlyCollection<UserId> Invited => invited.AsReadOnlyList();
    
    public IReadOnlyCollection<UserId> Attending => attending.AsReadOnlyList();

    public MeetupStatus Status { get; private set; } = MeetupStatus.Active;

    public bool HasOccurred => Date < SystemTime.Now;
    
    public bool IsCancelled => IsTerminal;
}