// tests/DevBoard.Api.Tests/HealthCheckTests.cs
using System.Net;
using Xunit;

namespace DevBoard.Api.Tests;

public sealed class HealthCheckTests : IClassFixture<DevBoardWebApplicationFactory>
{
    private readonly DevBoardWebApplicationFactory _factory;
    public HealthCheckTests(DevBoardWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task GET_Health_Returns200_WithDatabaseHealthy()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal("Healthy", body);
    }
}