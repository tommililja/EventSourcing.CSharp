using EventSourcing.Domain.Common;
using Xunit;

namespace EventSourcing.Test.Common;

public class SequenceNumberTests
{
    [Fact]
    public void SequenceNumber_FromInt_Should_Succeed()
    {
        var number = SequenceNumber
            .FromInt(0)
            .ToInt();
        
        Assert.Equal(0, number);
    }
    
    [Fact]
    public void SequenceNumber_FromInt_Should_Throw()
    {
        CustomAssert.Throws(() => SequenceNumber.FromInt(-1));
    }

    [Fact]
    public void SequenceNumber_Zero_Should_Be_Zero()
    {
        var number = SequenceNumber
            .Zero()
            .ToInt();
        
        Assert.Equal(0, number);
    }

    [Fact]
    public void SequenceNumber_SequenceNumber_Should_Equal()
    {
        var number1 = SequenceNumber.Zero();
        var number2 = SequenceNumber.FromInt(0);

        Assert.Equal(number1, number2);
    }
    
    [Fact]
    public void SequenceNumber_Increment_Should_Increment()
    {
        var number = SequenceNumber
            .Zero()
            .Increment()
            .ToInt();

        Assert.Equal(1, number);
    }
    
    [Fact]
    public void SequenceNumber_Should_Be_LesserThan()
    {
        var number = SequenceNumber.Zero();
        var number2 = SequenceNumber.FromInt(1);

        Assert.True(number < number2);
    }
    
    [Fact]
    public void SequenceNumber_Should_Be_LesserThanOrEqual()
    {
        var number = SequenceNumber.Zero();
        var number2 = SequenceNumber.FromInt(1);

        Assert.True(number <= number2);
    }
    
    [Fact]
    public void SequenceNumber_Should_Be_LargerThan()
    {
        var number = SequenceNumber.Zero();
        var number2 = SequenceNumber.FromInt(1);

        Assert.True(number2 > number);
    }
    
    [Fact]
    public void SequenceNumber_Should_Be_LargerThanOrEqual()
    {
        var number = SequenceNumber.Zero();
        var number2 = SequenceNumber.FromInt(1);

        Assert.True(number2 >= number);
    }
}