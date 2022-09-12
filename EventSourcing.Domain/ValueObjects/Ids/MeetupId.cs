using EventSourcing.Domain.Common;

namespace EventSourcing.Domain.ValueObjects.Ids;

public class MeetupId : AggregateId
{
    public MeetupId()
    {
            
    }
        
    private MeetupId(Guid id) : base(id)
    {
            
    }

    public static MeetupId Generate() => new();
    
    public static MeetupId FromGuid(Guid guid) => new(guid);
}