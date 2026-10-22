using GuardPost.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static GuardPost.Common.EntityValidationConstants.Incident;

namespace GuardPost.Data.Configurations;

public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(TitleMaxLength);

        builder.Property(i => i.Description)
            .IsRequired()
            .HasMaxLength(DescriptionMaxLength);

        builder.Property(i => i.ResolutionNotes)
            .HasMaxLength(ResolutionNotesMaxLength);

        builder.HasOne(i => i.Site)
            .WithMany(s => s.Incidents)
            .HasForeignKey(i => i.SiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.ReportedByGuard)
            .WithMany(g => g.ReportedIncidents)
            .HasForeignKey(i => i.ReportedByGuardId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(i => i.OccurredOn);

        builder.HasQueryFilter(i => !i.Site.IsDeleted);
    }
}
