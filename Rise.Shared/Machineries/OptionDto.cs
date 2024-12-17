using FluentValidation;

namespace Rise.Shared.Machineries;

public static class OptionDto
{

    public class Index
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
    }

    public class Detail
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public required CategoryDto.Index Category { get; set; }
    }

    public class Create
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int CategoryId { get; set; }
        public class Validator : AbstractValidator<Create>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty().WithMessage("Naam moet ingevuld zijn");
                RuleFor(x => x.Code).NotEmpty().WithMessage("Code moet ingevuld zijn");
            }
        }
    }
    public class Update
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public required int CategoryId { get; set; }

        public class Validator : AbstractValidator<Update>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty().WithMessage("Naam moet ingevuld zijn");
                RuleFor(x => x.Code).NotEmpty().WithMessage("Code moet ingevuld zijn");
            }
        }
    }
}