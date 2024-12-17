using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Translations;

namespace Rise.Persistence.Translations;

internal class TranslationConfiguration : EntityConfiguration<Translation>
{
    public override void Configure(EntityTypeBuilder<Translation> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.IsAccepted).IsRequired();
        builder.Property(x => x.OriginalText).HasMaxLength(255).IsRequired();
        builder.Property(x => x.TranslatedText).HasMaxLength(255).IsRequired();
	}
}
