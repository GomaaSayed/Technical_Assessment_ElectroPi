using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Technical_Assessment_ElectroPi.Core.Entities;

namespace Technical_Assessment_ElectroPi.Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TicketNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.TicketNumber)
            .IsUnique();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Priority)
            .IsRequired();

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.HasMany(x => x.Comments)
            .WithOne()
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Activities)
            .WithOne()
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.TimeEntries)
            .WithOne()
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.CustomerId);

        builder.HasIndex(x => x.AssignedAgentId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.Priority);

        builder.HasIndex(x => new
        {
            x.Status,
            x.Priority
        });
    }
}