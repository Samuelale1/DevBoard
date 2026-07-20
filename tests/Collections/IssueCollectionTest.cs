using DevBoard.Domain.Collections;
using DevBoard.Domain.Entities;
using DevBoard.Domain.Enums;
using DevBoard.Domain.ValueObjects;

namespace DevBoard.Domain.Tests.Collections;

public class IssueCollectionTests
{
    [Fact]
    public void IssueCollection_Add_ThenIndexByKey_ReturnsIssue()
    {
        // Arrange
        var collection = new IssueCollection();

        var issue = Issue.Create(
            "Login Bug",
            "User cannot login",
            IssueType.Bug,
            IssuePriority.High,
            "DEV-1",
            Guid.NewGuid());

        // Act
        collection.Add(issue);

        var result = collection["DEV-1"];

        // Assert
        Assert.NotNull(result);
        Assert.Equal(issue, result);
    }

    [Fact]
    public void IssueCollection_IndexMissingKey_ReturnsNull()
    {
        // Arrange
        var collection = new IssueCollection();

        // Act
        var result = collection["DEV-999"];

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void IssueCollection_IsEnumerable_CanForeach()
    {
        // Arrange
        var collection = new IssueCollection();

        collection.Add(Issue.Create(
            "Issue One",
            null,
            IssueType.Task,
            IssuePriority.Low,
            "DEV-1",
            Guid.NewGuid()));

        collection.Add(Issue.Create(
            "Issue Two",
            null,
            IssueType.Task,
            IssuePriority.Medium,
            "DEV-2",
            Guid.NewGuid()));

        // Act
        int count = 0;

        foreach (var issue in collection)
        {
            count++;
        }

        // Assert
        Assert.Equal(2, count);
    }
}