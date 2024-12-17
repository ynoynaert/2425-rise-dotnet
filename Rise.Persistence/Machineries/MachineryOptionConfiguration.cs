using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Machineries;

namespace Rise.Persistence.Machineries;

/// <summary>
/// Specific configuration for <see cref="MachineryOption"/>.
/// </summary>
internal class MachineryOptionConfiguration : EntityConfiguration<MachineryOption>
{

    public override void Configure(EntityTypeBuilder<MachineryOption> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Price).HasPrecision(18, 2).IsRequired();

        builder.HasOne(x => x.Machinery)
               .WithMany(x => x.MachineryOptions)
               .HasForeignKey("MachineryId")
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Option)
               .WithMany()
               .HasForeignKey("OptionId")
               .OnDelete(DeleteBehavior.Cascade);
    }
}

