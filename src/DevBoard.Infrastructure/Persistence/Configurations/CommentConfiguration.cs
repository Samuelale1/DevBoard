using DevBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBoard.Infrastructure.Persistence.Configurations;

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Content)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(c => c.IssueId)
               .IsRequired();

        builder.Property(c => c.AuthorId)
               .IsRequired();
    }
}