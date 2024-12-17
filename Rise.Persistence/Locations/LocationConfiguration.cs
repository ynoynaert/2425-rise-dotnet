using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Locations;

namespace Rise.Persistence.Locations;

internal class LocationConfiguration : EntityConfiguration<Location>
{
    public override void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Street).IsRequired();
        builder.Property(x => x.StreetNumber).IsRequired();
        builder.Property(x => x.City).IsRequired();
        builder.Property(x => x.PostalCode).IsRequired();
        builder.Property(x => x.Country).IsRequired();
        builder.Property(x => x.Image).IsRequired();
        builder.Property(x => x.PhoneNumber).IsRequired();
        builder.Property(x => x.VatNumber).IsRequired();
        builder.Property(x => x.Code).IsRequired();
    }
}
