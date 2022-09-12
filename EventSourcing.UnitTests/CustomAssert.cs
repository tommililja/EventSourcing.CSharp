using EventSourcing.Domain.Common;
using Xunit;

namespace EventSourcing.Test;

public static class CustomAssert
{
    public static void EventSequenceNumbers(IAggregate aggregate)
    {
        var versions = Enumerable
            .Range(0, aggregate.Changes.Count)
            .Select(SequenceNumber.FromInt);
        
        var missingVersion = aggregate.Changes
            .Select(x => x.SequenceNumber)
            .Except(versions)
            .Count();
        
        Assert.Equal(0, missingVersion);
    }

    public static void LastEvent<T>(IAggregate aggregate) => 
        Assert.IsType<T>(aggregate.Changes.Last());
    
    public static void Events(IAggregate aggregate, int events) => 
        Assert.Equal(events, aggregate.Changes.Count);

    public static void Throws(Action action) => 
        Assert.ThrowsAny<Exception>(action);
}