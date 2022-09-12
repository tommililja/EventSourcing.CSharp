using EventSourcing.Domain.Common;
using NodaTime;
using Xunit;

namespace EventSourcing.Test.Common;

public class SystemTimeTests
{
    [Fact]
    public void SystemTime_Now_Should_Return_Current_Time()
    {
        var currentInstant = SystemClock.Instance
            .GetCurrentInstant()
            .ToDateTimeOffset();
        
        var systemTime = SystemTime.Now
            .ToDateTimeOffset();
        
        Assert.Equal(currentInstant.Date, systemTime.Date);
    }
    
    [Fact]
    public void SystemTime_ChangeCurrentTime_Should_Change_Current_Time()
    {
        var lastYear = SystemTime.Now - Duration.FromDays(365);
        
        SystemTime.ChangeCurrentTime(lastYear);

        var now = SystemTime.Now;
        
        Assert.Equal(lastYear, now);
    }
    
    [Fact]
    public void SystemTime_Reset_Should_Reset_Current_Time()
    {
        var lastYear = SystemTime.Now - Duration.FromDays(365);
        
        SystemTime.ChangeCurrentTime(lastYear);
        SystemTime.Reset();

        var currentInstant = SystemClock.Instance
            .GetCurrentInstant()
            .ToDateTimeOffset();
        
        var systemTime = SystemTime.Now
            .ToDateTimeOffset();
        
        Assert.Equal(currentInstant.Date, systemTime.Date);
    }
}