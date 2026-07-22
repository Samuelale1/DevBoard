using DevBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Infrastructure.Persistence;

public sealed class AppDbContext
    : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Issue> Issues => Set<Issue>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<Comment> Comments => Set<Comment>();

    public DbSet<Label> Labels => Set<Label>();

    public DbSet<Webhook> Webhooks => Set<Webhook>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}