using DevBoard.Domain.ValueObjects;
public class SlugTests
{
    [Fact]
    public void From_ShouldGenerateSlug()
    {
        var slug = Slug.From("Hello World");

        Assert.Equal("hello-world", slug.Value);
    }
}