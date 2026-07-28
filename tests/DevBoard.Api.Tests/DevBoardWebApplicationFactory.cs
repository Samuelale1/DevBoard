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

namespace DevBoard.Api.Tests;

public sealed class DevBoardWebApplicationFactory : WebApplicationFactory<Program>
{
    public Guid AdminUserId { get; private set; }
    public Guid SeedProjectId { get; private set; }
    public Guid SeedWorkspaceId { get; } = Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove BOTH the options descriptor and the Npgsql configuration delegate —
            // AddDbContext appends IDbContextOptionsConfiguration<T> rather than replacing it,
            // so removing only DbContextOptions<AppDbContext> leaves Npgsql's config active.
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll(typeof(IDbContextOptionsConfiguration<AppDbContext>));

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase($"DevBoardTests-{Guid.NewGuid()}"));

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