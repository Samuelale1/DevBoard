namespace DevBoard.Domain.Exceptions;

public sealed class UnauthorizedException : DevBoardException
{
    public override int StatusCode => 403;
    public UnauthorizedException(string message) : base(message)
    {
        
    }
}
