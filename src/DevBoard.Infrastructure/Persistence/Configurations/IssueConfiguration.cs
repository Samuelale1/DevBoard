using DevBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBoard.Infrastructure.Persistence.Configurations;

public sealed class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Title)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(i => i.Description)
               .HasMaxLength(4000);

        builder.Property(i => i.IssueKey)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(i => i.Status)
               .IsRequired();

        builder.Property(i => i.Type)
               .IsRequired();

        builder.Property(i => i.ProjectId)
               .IsRequired();

        builder.Property(i => i.AssigneeId);

        builder.HasMany(i => i.Comments)
               .WithOne()
               .HasForeignKey(c => c.IssueId);

        builder.HasMany(i => i.Labels)
               .WithMany();
    }
}