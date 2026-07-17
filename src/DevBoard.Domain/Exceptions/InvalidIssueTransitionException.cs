namespace DevBoard.Domain.Exceptions;

using DevBoard.Domain.Enums;

public sealed class InvalidIssueTransitionException : DevBoardException
{
    public override int StatusCode => 409;

    public InvalidIssueTransitionException(
        IssueStatus from,
        IssueStatus to)
        : base($"Cannot transition an issue from '{from}' to '{to}'.")
    {
    }
}