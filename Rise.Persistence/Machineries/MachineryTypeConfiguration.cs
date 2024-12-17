using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Machineries;

namespace Rise.Persistence.Machineries;

/// <summary>
/// Specific configuration for <see cref="MachineryType"/>.
/// </summary>
internal class MachineryTypeConfiguration : EntityConfiguration<MachineryType>
{
    public override void Configure(EntityTypeBuilder<MachineryType> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.HasMany(x => x.Machineries)
            .WithOne(x => x.Type)
            .HasForeignKey("MachineryTypeId");

        builder.HasIndex(x => x.Name).IsUnique();
    }
}
