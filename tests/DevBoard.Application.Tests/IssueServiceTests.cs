// tests/DevBoard.Application.Tests/IssueServiceTests.cs
using DevBoard.Application.Services.Implementations;
using DevBoard.Domain.Entities;
using DevBoard.Domain.Enums;
using DevBoard.Domain.Exceptions;
using DevBoard.Domain.Interfaces;
using DevBoard.Domain.ValueObjects;
using DevBoard.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevBoard.Application.Tests;

public sealed class IssueServiceTests
{
    private readonly Mock<IRepository<Issue>> _issueRepo = new();
    private readonly Mock<IRepository<Project>> _projectRepo = new();
    private readonly Mock<ILogger<IssueService>> _logger = new();
    private readonly Mock<IHubContext<BoardHub>> _hub = new();
    private readonly Mock<IClientProxy> _clientProxy = new();

    private IssueService CreateSut()
    {
        var clients = new Mock<IHubClients>();
        clients.Setup(c => c.Group(It.IsAny<string>())).Returns(_clientProxy.Object);
        _hub.Setup(h => h.Clients).Returns(clients.Object);

        return new IssueService(_issueRepo.Object, _projectRepo.Object, _logger.Object, _hub.Object);
    }

    [Fact]
    public async Task ChangeStatus_ValidTransition_SavesChangesAndNotifiesHub()
    {
        var issue = Issue.Create("Test bug", null, IssueType.Bug, IssuePriority.Low, "T-1", Guid.NewGuid());
        _issueRepo.Setup(r => r.GetByIdAsync(issue.Id, default)).ReturnsAsync(issue);

        var sut = CreateSut();
        await sut.ChangeStatusAsync(issue.Id, IssueStatus.Todo, default);

        _issueRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        _clientProxy.Verify(c => c.SendCoreAsync("IssueUpdated", It.IsAny<object[]>(), default), Times.Once);
    }

    [Fact]
    public async Task ChangeStatus_IssueNotFound_ThrowsNotFoundException()
    {
        _issueRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Issue?)null);

        var sut = CreateSut();

        await Assert.ThrowsAsync<NotFoundException>(
            () => sut.ChangeStatusAsync(Guid.NewGuid(), IssueStatus.Todo, default));
    }

    [Fact]
    public async Task CreateIssue_SetsIssueKey_FromProjectCounter()
    {
        var project = Project.Create("Test Project", Slug.From("Test Project"), Guid.NewGuid());
        _projectRepo.Setup(r => r.GetByIdAsync(project.Id, default)).ReturnsAsync(project);

        var sut = CreateSut();
        var issue = await sut.CreateAsync(project.Id, "New bug", null, IssueType.Bug, IssuePriority.Medium, default);

        Assert.Equal("TEST-PROJECT-1", issue.IssueKey);
        _projectRepo.Verify(r => r.Update(project), Times.Once);
    }
}