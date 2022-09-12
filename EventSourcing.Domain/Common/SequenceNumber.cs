using Light.GuardClauses;

namespace EventSourcing.Domain.Common;

public class SequenceNumber : ValueObject, IComparable<SequenceNumber>
{
    private int number;

    private SequenceNumber()
    {
        number = 0;
    }

    private SequenceNumber(int number)
    {
        number.MustBeGreaterThanOrEqualTo(0, message:
            "Sequence number must be greater or equal to 0.");
        
        this.number = number;
    }

    public static bool operator <(SequenceNumber a, SequenceNumber b) => a.ToInt() < b.ToInt();

    public static bool operator <=(SequenceNumber a, SequenceNumber b) => a.ToInt() <= b.ToInt();

    public static bool operator >(SequenceNumber a, SequenceNumber b) => a.ToInt() > b.ToInt();

    public static bool operator >=(SequenceNumber a, SequenceNumber b) => a.ToInt() >= b.ToInt();

    protected override IEnumerable<object> Values => new List<object> { number };

    public SequenceNumber Increment()
    {
        number++;

        return this;
    }
    
    public int CompareTo(SequenceNumber? other)
    {
        return number.CompareTo(other?.number);
    }

    public int ToInt() => number;

    public static SequenceNumber Zero() => new();

    public static SequenceNumber FromInt(int number) => new(number);
}