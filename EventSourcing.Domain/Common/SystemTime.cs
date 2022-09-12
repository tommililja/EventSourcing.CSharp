using NodaTime;

namespace EventSourcing.Domain.Common;

public static class SystemTime
{
    private static readonly Func<Instant> DefaultTime = () => SystemClock.Instance.GetCurrentInstant();

    private static Func<Instant> getSystemTime = DefaultTime;

    public static Instant Now => getSystemTime();
    
    public static void ChangeCurrentTime(Instant instant) => getSystemTime = () => instant;
    
    public static void Reset() => getSystemTime = DefaultTime;
}