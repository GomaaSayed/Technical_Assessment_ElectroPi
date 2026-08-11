using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Technical_Assessment_ElectroPi.Core.Entities;

namespace Technical_Assessment_ElectroPi.Infrastructure.Persistence.Configurations;

public class TimeEntryConfiguration
    : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("TimeEntries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.WorkDate)
            .IsRequired();

        builder.Property(x => x.DurationMinutes)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.TicketId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.HasIndex(x => x.TicketId);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => new
        {
            x.TicketId,
            x.WorkDate
        });
    }
}