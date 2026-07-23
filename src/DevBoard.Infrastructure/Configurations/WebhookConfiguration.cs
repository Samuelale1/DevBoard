using DevBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBoard.Infrastructure.Persistence.Configurations;

public sealed class WebhookConfiguration : IEntityTypeConfiguration<Webhook>
{
    public void Configure(EntityTypeBuilder<Webhook> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.TargetUrl)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(w => w.WorkspaceId)
               .IsRequired();

        builder.Ignore(w => w.Events);
    }
}