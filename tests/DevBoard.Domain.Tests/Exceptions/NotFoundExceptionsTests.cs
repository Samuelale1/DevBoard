using DevBoard.Domain.Exceptions;
using Xunit;

namespace DevBoard.Domain.Tests.Exceptions;

public class NotFoundExceptionTests
{
    [Fact]
    public void NotFoundException_IsDevBoardException()
    {
        // Arrange
        var ex = new NotFoundException("Issue not found");

        // Assert
        Assert.IsAssignableFrom<DevBoardException>(ex);
        Assert.Equal(404, ex.StatusCode);
    }
}