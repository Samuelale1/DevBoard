namespace DevBoard.Domain.Exceptions;

public  sealed class ConflictException : DevBoardException
{
    public override int StatusCode => 409;
    public ConflictException(string message) : base(message)
    {
    }
}