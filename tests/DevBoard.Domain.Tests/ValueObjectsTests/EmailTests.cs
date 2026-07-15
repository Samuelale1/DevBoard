using DevBoard.Domain.ValueObjects;
public class EmailTests
{
    [Fact]
    public void From_ShouldNormalizeEmail()
    {
        var email = Email.From("SAM@Test.COM");

        Assert.Equal("sam@test.com", email.Value);
    }

    [Fact]
    public void From_ShouldThrow_WhenInvalid()
    {
        Assert.Throws<ArgumentException>(() =>
            Email.From("notanemail"));
    }
}