using FluentValidation;
using Rise.Shared.Quotes;

namespace Rise.Shared.Machineries;

public static class MachineryOptionDto
{
    public class Index
    {
		public int Id { get; set; }
		public int MachineryId { get; set; }
		public int OptionId { get; set; }
		public required decimal Price { get; set; }
    }

    public class Detail
    {
        public int Id { get; set; }
        public required MachineryDto.Index Machinery { get; set; }
        public required OptionDto.Index Option { get; set; }
        public required decimal Price { get; set; }

    }


    public class XtremeDetail
    {
        public int Id { get; set; }
        public required OptionDto.Detail Option { get; set; }
        public required decimal Price { get; set; }
    }


    public class Create
    {
        public int MachineryId { get; set; }
        public int OptionId { get; set; }
        public decimal Price { get; set; }

        public class Validator : AbstractValidator<Create>
		{
			public Validator()
			{
				RuleFor(x => x.OptionId).NotEmpty().WithMessage("Optie moet ingevuld zijn");
                RuleFor(x => x.Price).GreaterThan(0.01M).WithMessage("Prijs moet groter dan 0.01 zijn");
            }
        }
	}

    public class Update
    {
        public required int Id { get; set; }
        public required int MachineryId { get; set; }
        public required int OptionId { get; set; }
        public required decimal Price { get; set; }

        public class Validator : AbstractValidator<Update>
		{
			public Validator()
			{
				RuleFor(x => x.Id).NotEmpty().WithMessage("Id moet ingevuld zijn");
				RuleFor(x => x.MachineryId).NotEmpty().WithMessage("MachineryId moet ingevuld zijn");
				RuleFor(x => x.OptionId).NotEmpty().WithMessage("OptionId moet ingevuld zijn");
                RuleFor(x => x.Price).GreaterThan(0.01M).WithMessage("Prijs moet groter dan 0.01 zijn");
            }
		}

	}


}

