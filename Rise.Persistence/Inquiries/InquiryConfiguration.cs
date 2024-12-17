using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Inquiries;

namespace Rise.Persistence.Inquiries;

public class InquiryConfiguration
{
    public void Configure(EntityTypeBuilder<Inquiry> builder)
    {
        builder.Property(x => x.CustomerName).IsRequired();
        builder.Property(x => x.SalespersonId).IsRequired();

        builder.HasOne(x => x.Machinery)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasMany(x => x.InquiryOptions)
            .WithOne()
            .HasForeignKey("InquiryId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
