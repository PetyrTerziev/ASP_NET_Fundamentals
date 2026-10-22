using GuardPost.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static GuardPost.Common.EntityValidationConstants.Shift;

namespace GuardPost.Data.Configurations;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Notes)
            .HasMaxLength(NotesMaxLength);

        builder.HasOne(s => s.Site)
            .WithMany(site => site.Shifts)
            .HasForeignKey(s => s.SiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Guard)
            .WithMany(g => g.Shifts)
            .HasForeignKey(s => s.GuardId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(s => new { s.GuardId, s.StartTime });

        builder.HasIndex(s => new { s.SiteId, s.StartTime });
       
        builder.HasQueryFilter(s => !s.Site.IsDeleted && !s.Guard.IsDeleted);
    }
}
