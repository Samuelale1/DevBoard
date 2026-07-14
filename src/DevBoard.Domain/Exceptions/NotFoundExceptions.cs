namespace DevBoard.Domain.Exceptions;

public class NotFoundException : DevBoardException
{
    public override int StatusCode => 404;

    public NotFoundException(string message)
        : base(message)
    {
    }
}