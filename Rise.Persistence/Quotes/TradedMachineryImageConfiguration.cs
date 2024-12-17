using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Quotes;

namespace Rise.Persistence.Quotes;

internal class TradedMachineryImageConfiguration : EntityConfiguration<TradedMachineryImage>
{
    public override void Configure(EntityTypeBuilder<TradedMachineryImage> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Url).IsRequired();
        builder.HasOne(x => x.TradedMachinery)
               .WithMany(x => x.Images)
               .HasForeignKey("TradedMachineryId");
    }
}
