using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Machineries;

namespace Rise.Persistence.Machineries;

/// <summary>
/// Specific configuration for <see cref="Category"/>.
/// </summary>
internal class CategoryConfiguration : EntityConfiguration<Category>
{

    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(10).IsRequired();

        builder.HasIndex(x => x.Code).IsUnique();
    }
}

