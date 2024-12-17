using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Machineries;

namespace Rise.Persistence.Machineries;

internal class ImageConfiguration : EntityConfiguration<Image>
{
	public override void Configure(EntityTypeBuilder<Image> builder)
	{
		base.Configure(builder);
		builder.Property(x => x.Url).IsRequired();
		builder.HasOne(x => x.Machinery)
			   .WithMany(x => x.Images)
			   .HasForeignKey("MachineryId");
	}
}
