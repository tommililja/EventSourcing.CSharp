namespace EventSourcing.Domain.Common;

public abstract class ValueObject
{
    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        var valueObject = (ValueObject)obj;

        return Values.SequenceEqual(valueObject.Values);
    }

    public override int GetHashCode()
    {
        var hashCode = Values.Aggregate(1, (current, obj) =>
        {
            unchecked
            {
                return current * 23 + obj.GetHashCode();
            }
        });

        return hashCode;
    }
    
    public static bool operator ==(ValueObject a, ValueObject b) => a.Equals(b);

    public static bool operator !=(ValueObject a, ValueObject b) => !a.Equals(b);

    protected abstract IEnumerable<object> Values { get; }
}