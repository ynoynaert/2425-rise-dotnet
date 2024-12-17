using FluentValidation;

namespace Rise.Shared.Machineries;

public static class MachineryDto
{
    public class Index
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string SerialNumber { get; set; }
        public required MachineryTypeDto.Index Type { get; set; }
        public required string Description { get; set; }
    }

    public class Detail
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string SerialNumber { get; set; }
        public required MachineryTypeDto.Index Type { get; set; }
        public required string Description { get; set; }
        public required string BrochureText { get; set; }
        public required List<ImageDto.Index> Images { get; set; } = [];
        public required List<MachineryOptionDto.Index> MachineryOptions { get; set; } = [];
    }

    public class Create
    {
        public string? Name { get; set; }
        public string? SerialNumber { get; set; }
        public int TypeId { get; set; }
        public string? Description { get; set; }
        public string? BrochureText { get; set; }
        public List<string> ImageContentType { get; set; } = [];
		public class Validator : AbstractValidator<Create>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty().WithMessage("Naam moet ingevuld zijn");
                RuleFor(x => x.SerialNumber).NotEmpty().WithMessage("Serienummer moet ingevuld zijn");
                RuleFor(x => x.TypeId).NotEmpty().WithMessage("Type moet ingevuld zijn");
                RuleFor(x => x.Description).NotEmpty().WithMessage("Beschrijving moet ingevuld zijn");
                RuleFor(x => x.ImageContentType).Must(images => images != null && images.Any()).WithMessage("Er moet minstens één afbeelding gekozen zijn.");
                RuleFor(x => x.BrochureText).NotEmpty().WithMessage("Brochure tekst moet ingevuld zijn");
			}
        }
    }

	public class Update
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? SerialNumber { get; set; }
		public int TypeId { get; set; }
		public string? Description { get; set; }
        public string? BrochureText { get; set; }
        public List<string> ImageContentTypeNew { get; set; } = [];
		public List<string> urlOld { get; set; } = [];

		public class Validator : AbstractValidator<Update>
		{
			public Validator()
			{
				RuleFor(x => x.Name).NotEmpty().WithMessage("Naam moet ingevuld zijn");
				RuleFor(x => x.SerialNumber).NotEmpty().WithMessage("Serienummer moet ingevuld zijn");
				RuleFor(x => x.TypeId).NotEmpty().WithMessage("Type moet ingevuld zijn");
				RuleFor(x => x.Description).NotEmpty().WithMessage("Beschrijving moet ingevuld zijn");
				RuleFor(x => x.BrochureText).NotEmpty().WithMessage("Brochure tekst moet ingevuld zijn");

				RuleFor(x => x.ImageContentTypeNew)
				.NotEmpty()
				.WithMessage("Er moet minstens één nieuwe afbeelding gekozen zijn.")
				.When(x => x.urlOld == null || !x.urlOld.Any());
			}
		}
	
    }

    public class XtremeDetail
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string SerialNumber { get; set; }
        public required MachineryTypeDto.Index Type { get; set; }
        public required string Description { get; set; }
        public required List<MachineryOptionDto.XtremeDetail> MachineryOptions { get; set; } = [];
        public required List<ImageDto.Index> Images { get; set; }
		public required string BrochureText { get; set; }
    }
}

