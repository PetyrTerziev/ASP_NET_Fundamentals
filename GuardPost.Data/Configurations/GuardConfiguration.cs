using GuardPost.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static GuardPost.Common.EntityValidationConstants.Guard;
using static GuardPost.Common.EntityValidationConstants.Shared;

namespace GuardPost.Data.Configurations;

public class GuardConfiguration : IEntityTypeConfiguration<Guard>
{
    public void Configure(EntityTypeBuilder<Guard> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.FirstName)
            .IsRequired()
            .HasMaxLength(FirstNameMaxLength);

        builder.Property(g => g.LastName)
            .IsRequired()
            .HasMaxLength(LastNameMaxLength);

        builder.Property(g => g.LicenseNumber)
            .IsRequired()
            .HasMaxLength(LicenseNumberMaxLength);

        builder.Property(g => g.PhoneNumber)
            .IsRequired()
            .HasMaxLength(PhoneMaxLength);

        builder.Property(g => g.Email)
            .HasMaxLength(EmailMaxLength);

        builder.Property(g => g.HireDate)
            .HasColumnType("date");

        builder.HasIndex(g => g.LicenseNumber)
            .IsUnique();

        builder.HasQueryFilter(g => !g.IsDeleted);
    }
}
