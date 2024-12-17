using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Orders;

namespace Rise.Persistence.Orders;

internal class OrderConfiguration : EntityConfiguration<Order>
{
    public override void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(x => x.OrderNumber).IsRequired();
        builder.HasIndex(x => x.OrderNumber).IsUnique();  
        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.IsCancelled).IsRequired();

        builder.HasOne(x => x.Quote)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();
    }

}
