using DevBoard.Domain.Enums;
using DevBoard.Domain.Entities;

public class IssueTests
{
    [Fact]
    public void NewIssue_ShouldStartInBacklog()
    {
        var issue = new Issue
        {
            Title = "Login Bug",
            Type = IssueType.Bug,
            IssueKey = "DEV-1",
            ProjectId = Guid.NewGuid()
        };

        Assert.Equal(IssueStatus.Backlog, issue.Status);
    }
}