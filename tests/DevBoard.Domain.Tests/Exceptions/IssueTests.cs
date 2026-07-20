using DevBoard.Domain.Enums;
using DevBoard.Domain.Entities;
using DevBoard.Domain.ValueObjects;

public class IssueTests
{
    [Fact]
    public void NewIssue_ShouldStartInBacklog()
    {
        var issue = Issue.Create(
            "Bug",
            null,
            IssueType.Bug,
            IssuePriority.High,
            "DEV-1",
            Guid.NewGuid()
            );
        
        Assert.Equal(IssueStatus.Backlog, issue.Status);
    }
}