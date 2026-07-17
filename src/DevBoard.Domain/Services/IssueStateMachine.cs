namespace DevBoard.Domain.Services;

using DevBoard.Domain.Enums;

public static class IssueStateMachine
{
    public static bool IsValidTransition(
        IssueStatus from,
        IssueStatus to)
    {
        return (from, to) switch
        {
            (IssueStatus.Backlog, IssueStatus.Todo) => true,

            (IssueStatus.Todo, IssueStatus.InProgress) => true,

            (IssueStatus.InProgress, IssueStatus.InReview) => true,

            (IssueStatus.InReview, IssueStatus.Done) => true,

            (IssueStatus.Backlog, IssueStatus.Cancelled) => true,

            (IssueStatus.Todo, IssueStatus.Cancelled) => true,

            (IssueStatus.InProgress, IssueStatus.Cancelled) => true,

            (IssueStatus.InReview, IssueStatus.Cancelled) => true,

            _ => false
        };
    }
}