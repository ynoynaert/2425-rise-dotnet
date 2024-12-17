using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Quotes;

namespace Rise.Persistence.Quotes;
internal class QuoteConfiguration : EntityConfiguration<Quote>
{
    public override void Configure(EntityTypeBuilder<Quote> builder)
    {
        builder.Property(x => x.QuoteNumber).IsRequired();
        builder.HasIndex(x => x.QuoteNumber).IsUnique();
        builder.Property(x => x.IsApproved).IsRequired();
        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.SalespersonId).IsRequired();
        builder.Property(x => x.TotalWithoutVat).IsRequired();
        builder.Property(x => x.TotalWithVat).IsRequired();

        builder.HasOne(x => x.Machinery)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.Customer)
           .WithMany()
           .OnDelete(DeleteBehavior.Restrict)
           .IsRequired();

        builder.HasMany(x => x.TradedMachineries)
                 .WithOne(x => x.Quote)
                 .OnDelete(DeleteBehavior.Restrict);
    }
}
