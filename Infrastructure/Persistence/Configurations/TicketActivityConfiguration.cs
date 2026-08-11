using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Technical_Assessment_ElectroPi.Core.Entities;

namespace Technical_Assessment_ElectroPi.Infrastructure.Persistence.Configurations;

public class TicketActivityConfiguration
    : IEntityTypeConfiguration<TicketActivity>
{
    public void Configure(EntityTypeBuilder<TicketActivity> builder)
    {
        builder.ToTable("TicketActivities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ActivityType)
            .IsRequired();

        builder.Property(x => x.OldValue)
            .HasMaxLength(500);

        builder.Property(x => x.NewValue)
            .HasMaxLength(500);

        builder.Property(x => x.TicketId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.TicketId);

        builder.HasIndex(x => new
        {
            x.TicketId,
            x.CreatedAt
        });
    }
}