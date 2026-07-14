namespace DevBoard.Domain.Exceptions;

public abstract class DevBoardException : Exception
{
      public abstract int StatusCode { get; }
    protected DevBoardException(string message)
        : base(message)
    {
        
    }
}