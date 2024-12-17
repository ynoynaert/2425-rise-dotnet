using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Machineries;

public class MachineryTypeDto
{
    public class Index
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
    public class Create
    {
        public string? Name { get; set; }
        public class Validator : AbstractValidator<Create>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty().WithMessage("Naam moet ingevuld zijn");
            }
        }
    }
    public class Update
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public class Validator : AbstractValidator<Update>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty().WithMessage("Naam moet ingevuld zijn");
            }
        }
    }
}
