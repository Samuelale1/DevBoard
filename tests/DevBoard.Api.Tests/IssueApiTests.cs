// tests/DevBoard.Api.Tests/IssueApiTests.cs
using System.Net;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DevBoard.Domain.Enums;
using Xunit;

namespace DevBoard.Api.Tests;

public sealed class IssueApiTests : IClassFixture<DevBoardWebApplicationFactory>
{
    private readonly DevBoardWebApplicationFactory _factory;

    public IssueApiTests(DevBoardWebApplicationFactory factory) => _factory = factory;

    private async Task<HttpClient> AuthenticatedClientAsync()
    {
        var client = _factory.CreateClient();
        var token = await client.LoginAsAdminAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    [Fact]
    public async Task POST_CreateIssue_Returns201_WithProjectKey()
    {
        var client = await AuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/issues", new
        {
            ProjectId = _factory.SeedProjectId,
            Title = "Integration test bug",
            Description = (string?)null,
            Type = IssueType.Bug,
            Priority = 2
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Matches(@"^[A-Z0-9-]+-\d+$", body.GetProperty("issueKey").GetString());
    }

    [Fact]
    public async Task GET_Issues_Paginated_ReturnsCorrectPage()
    {
        var client = await AuthenticatedClientAsync();

        for (var i = 0; i < 3; i++)
        {
            await client.PostAsJsonAsync("/api/issues", new
            {
                ProjectId = _factory.SeedProjectId,
                Title = $"Bulk issue {i}",
                Description = (string?)null,
                Type = IssueType.Task,
                Priority = 1
            });
        }

        var response = await client.GetAsync($"/api/issues?projectId={_factory.SeedProjectId}&page=1&pageSize=2");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(2, body.GetProperty("items").GetArrayLength());
    }

    [Fact]
    public async Task PATCH_ChangeStatus_ValidTransition_Returns200()
    {
        var client = await AuthenticatedClientAsync();

        var createResponse = await client.PostAsJsonAsync("/api/issues", new
        {
            ProjectId = _factory.SeedProjectId,
            Title = "Status transition test",
            Description = (string?)null,
            Type = IssueType.Bug,
            Priority = 1
        });
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var issueId = created.GetProperty("id").GetGuid();

        var response = await client.PatchAsJsonAsync($"/api/issues/{issueId}/status", new { NewStatus = IssueStatus.Todo });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PATCH_ChangeStatus_InvalidTransition_Returns422()
    {
        var client = await AuthenticatedClientAsync();

        var createResponse = await client.PostAsJsonAsync("/api/issues", new
        {
            ProjectId = _factory.SeedProjectId,
            Title = "Invalid transition test",
            Description = (string?)null,
            Type = IssueType.Bug,
            Priority = 1
        });
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var issueId = created.GetProperty("id").GetGuid();

        // Backlog -> Done is not a legal transition in your state machine
        var response = await client.PatchAsJsonAsync($"/api/issues/{issueId}/status", new { NewStatus = IssueStatus.Done });

        Assert.Equal((HttpStatusCode)422, response.StatusCode);
    }

    [Fact]
    public async Task GET_Issues_WithoutAuth_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/issues?projectId={_factory.SeedProjectId}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GET_AuditLog_ReturnsStreamedEntries()
    {
        var client = await AuthenticatedClientAsync();

        var createResponse = await client.PostAsJsonAsync("/api/issues", new
        {
            ProjectId = _factory.SeedProjectId,
            Title = "Audit log test",
            Description = (string?)null,
            Type = IssueType.Bug,
            Priority = 1
        });
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var issueId = created.GetProperty("id").GetGuid();

        await client.PatchAsJsonAsync($"/api/issues/{issueId}/status", new { NewStatus = IssueStatus.Todo });

        var response = await client.GetAsync($"/api/issues/{issueId}/audit-log");
        response.EnsureSuccessStatusCode();

        var entries = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(entries.GetArrayLength() > 0);
    }
}