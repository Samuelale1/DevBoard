
namespace DevBoard.Domain.ValueObjects;
public class IssuePriorityTests
{
    [Fact]
    public void Urgent_ShouldBeGreaterThan_High()
    {
        Assert.True(IssuePriority.Urgent > IssuePriority.High);
    }

    [Fact]
    public void Low_ShouldBeLessThan_Medium()
    {
        Assert.True(IssuePriority.Low < IssuePriority.Medium);
    }

    [Fact]
    public void CompareTo_ShouldReturnPositive()
    {
        Assert.True(
            IssuePriority.High.CompareTo(IssuePriority.Low) > 0);
    }
}