using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Machineries;

namespace Rise.Persistence.Machineries;

/// <summary>
/// Specific configuration for <see cref="Machinery"/>.
/// </summary>
internal class MachineryConfiguration : EntityConfiguration<Machinery>
{

    public override void Configure(EntityTypeBuilder<Machinery> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name).HasMaxLength(255).IsRequired();
        builder.Property(x => x.SerialNumber).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(255).IsRequired();
        builder.Property(x => x.BrochureText).IsRequired();

        builder.HasIndex(x => x.SerialNumber).IsUnique();
    }
}

