using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Machineries;

namespace Rise.Persistence.Machineries;

/// <summary>
/// Specific configuration for <see cref="Option"/>.
/// </summary>
internal class OptionConfiguration : EntityConfiguration<Option>
{

    public override void Configure(EntityTypeBuilder<Option> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(255).IsRequired();

        builder.HasOne(x => x.Category)
               .WithMany(x => x.Options)
               .HasForeignKey("CategoryId");

        builder.HasIndex(x => x.Code).IsUnique();
    }
}

