using EventSourcing.Domain.Common;
using Light.GuardClauses;

namespace EventSourcing.Domain.ValueObjects;

public class MeetupStatus : ValueObject
{
    protected override IEnumerable<object> Values => new List<object>();

    public static MeetupActiveStatus Active => new();

    public static MeetupPlannedStatus Planned => new();
    
    public static MeetupCancelledStatus Cancelled(string reason) => MeetupCancelledStatus.Create(reason);
}

public class MeetupPlannedStatus : MeetupStatus
{
    protected override IEnumerable<object> Values => new List<object>
    {
        nameof(MeetupPlannedStatus)
    };
}

public class MeetupActiveStatus : MeetupStatus
{
    protected override IEnumerable<object> Values => new List<object>
    {
        nameof(MeetupActiveStatus)
    };
}

public class MeetupCancelledStatus : MeetupStatus
{
    private readonly string reason;
    
    private MeetupCancelledStatus(string reason)
    {
        reason.Length.MustBeGreaterThanOrEqualTo(10, message: 
            "Reason must be at least 10 characters.");

        reason.Length.MustBeLessThanOrEqualTo(100, message: 
            "Reason must be less than 100 characters.");
        
        this.reason = reason;
    }
    
    protected override IEnumerable<object> Values
    {
        get { yield return reason; }
    }

    public override string ToString() => reason;

    public static MeetupCancelledStatus Create(string reason) => new(reason);
}