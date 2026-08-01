// tests/DevBoard.Api.Tests/DevBoardWebApplicationFactory.cs
using DevBoard.Domain.Entities;
using DevBoard.Domain.Enums;
using DevBoard.Domain.ValueObjects;
using DevBoard.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;

namespace DevBoard.Api.Tests;

public sealed class DevBoardWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"DevBoardTests-{Guid.NewGuid()}";   // would be generated ONCE

    public Guid AdminUserId { get; private set; }
    public Guid SeedProjectId { get; private set; }
    public Guid SeedWorkspaceId { get; } = Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "test-secret-key-at-least-32-characters-long-for-hmac-sha256",
                ["Jwt:Issuer"] = "https://localhost-test",
                ["Jwt:Audience"] = "devboard-client-test"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll(typeof(IDbContextOptionsConfiguration<AppDbContext>));

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));   
                
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            var project = Project.Create("Test Project", Slug.From("Test Project"), SeedWorkspaceId);
            var admin = User.Create(
                Email.From("admin@devboard.test"),
                "Admin User",
                BCrypt.Net.BCrypt.HashPassword("Password123!"),
                UserRole.Admin,
                SeedWorkspaceId);

            db.Projects.Add(project);
            db.Users.Add(admin);
            db.SaveChanges();

            AdminUserId = admin.Id;
            SeedProjectId = project.Id;
        });
    }
}