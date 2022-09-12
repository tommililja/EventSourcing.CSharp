using EventSourcing.Domain.ValueObjects.Ids;
using Xunit;

namespace EventSourcing.Test.ValueObjects.Ids;

public class UserIdTests
{
    [Fact]
    public void UserId_Create_Should_Succeed()
    {
        var guid = Guid.NewGuid();

        var userId = UserId
            .FromGuid(guid)
            .ToGuid();

        Assert.Equal(userId, guid);
    }
    
    [Fact]
    public void UserId_Create_Empty_Should_Throw()
    {
        var guid = Guid.Empty;

        CustomAssert.Throws(() => UserId.FromGuid(guid));
    }
    
    [Fact]
    public void UserId_Create_Empty_Should_Not_Equal_Empty_Guid()
    {
        var userId = UserId
            .Generate()
            .ToGuid();
        
        Assert.NotEqual(userId, Guid.Empty);
    }
    
    [Fact]
    public void UserId_Should_Equal()
    {
        var guid = Guid.NewGuid();

        var userId = UserId.FromGuid(guid);
        var userId2 = UserId.FromGuid(guid);

        Assert.True(userId == userId2);
    }
    
    [Fact]
    public void UserId_Should_Not_Equal()
    {
        var userId = UserId.Generate();
        var userId2 = UserId.Generate();

        Assert.True(userId != userId2);
    }
}