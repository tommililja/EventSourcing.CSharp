using EventSourcing.Domain.Common;
using Light.GuardClauses;

namespace EventSourcing.Domain.ValueObjects;

public class MeetupName : ValueObject
{
    private readonly string name;
    
    private MeetupName(string name)
    {
        name.Length.MustBeGreaterThanOrEqualTo(2, 
            message: "Name is too short.");
        
        name.Length.MustBeLessThanOrEqualTo(25, 
            message: "Name is too long.");

        this.name = name;
    }
    
    protected override IEnumerable<object> Values
    {
        get { yield return name; }
    }

    public override string ToString() => name;
    
    public static MeetupName Create(string name) => new(name);
}