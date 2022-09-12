using EventSourcing.Domain.Common;

namespace EventSourcing.Domain.ValueObjects.Ids;

public class UserId : AggregateId
{
    public UserId()
    {
            
    }
        
    private UserId(Guid id) : base(id)
    {
            
    }
    
    public static UserId Generate() => new();
    
    public static UserId FromGuid(Guid guid) => new(guid);
}