using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Inquiries;

namespace Rise.Persistence.Inquiries;

public class InquiryOptionConfiguration
{
    public void Configure(EntityTypeBuilder<InquiryOption> builder)
    {
        builder.HasOne(x => x.Inquiry)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict)
            .HasForeignKey("InquiryId")
            .IsRequired();

        builder.HasOne(x => x.MachineryOption)
            .WithMany()
            .HasForeignKey("MachineryOptionId")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
