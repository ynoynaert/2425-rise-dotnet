using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Quotes;

namespace Rise.Persistence.Quotes;

internal class TradedMachineryConfiguration : EntityConfiguration<TradedMachinery>
{
    public override void Configure(EntityTypeBuilder<TradedMachinery> builder)
    {
        builder.Property(x=> x.Name).IsRequired();
        builder.Property(x => x.SerialNumber).IsRequired();
        builder.Property(x => x.Description).IsRequired();
        builder.Property(x => x.EstimatedValue).IsRequired();
        builder.Property(x => x.Year).IsRequired();

        builder.HasOne(x => x.Quote)
             .WithMany(q => q.TradedMachineries)
             .HasForeignKey("QuoteId")
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired();

        builder.HasOne(builder => builder.Type)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
