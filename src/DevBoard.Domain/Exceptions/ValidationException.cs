namespace DevBoard.Domain.Exceptions;

public  sealed class ValidationException : DevBoardException
{
    public override int StatusCode => 422;
    public ValidationException(string message) : base(message)
    {
        
    }
}