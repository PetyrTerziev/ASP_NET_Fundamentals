using GuardPost.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static GuardPost.Common.EntityValidationConstants.Shared;
using static GuardPost.Common.EntityValidationConstants.Site;

namespace GuardPost.Data.Configurations;

public class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(NameMaxLength);

        builder.Property(s => s.Address)
            .IsRequired()
            .HasMaxLength(AddressMaxLength);

        builder.Property(s => s.City)
            .IsRequired()
            .HasMaxLength(CityMaxLength);

        builder.Property(s => s.ClientName)
            .IsRequired()
            .HasMaxLength(ClientNameMaxLength);

        builder.Property(s => s.ContactPhone)
            .IsRequired()
            .HasMaxLength(PhoneMaxLength);

        builder.Property(s => s.Description)
            .HasMaxLength(DescriptionMaxLength);

        builder.HasIndex(s => s.City);

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
