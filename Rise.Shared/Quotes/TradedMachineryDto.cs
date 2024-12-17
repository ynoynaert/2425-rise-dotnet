using FluentValidation;
using Rise.Shared.Machineries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Quotes;

public static class TradedMachineryDto
{
    public class Index
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required MachineryTypeDto.Index MachineryType { get; set; }
        public required string SerialNumber { get; set; }
        public required string Description { get; set; }
        public required decimal EstimatedValue { get; set; }
        public required int Year { get; set; }
        public required List<ImageDto.Index> Images { get; set; } = [];
    }

    public class Create
    {
        public  string? Name { get; set; }
        public  int TypeId { get; set; }
        public  string? SerialNumber { get; set; }
        public  string? Description { get; set; }
        public  decimal EstimatedValue { get; set; }
        public  int Year { get; set; }
        public  string? QuoteNumber { get; set; }
        public List<string> ImageContentType { get; set; } = [];
        public class Validator : AbstractValidator<Create>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty().WithMessage("Naam moet ingevuld zijn");
                RuleFor(x => x.TypeId).NotEmpty().WithMessage("Type moet ingevuld zijn");
                RuleFor(x => x.SerialNumber).NotEmpty().WithMessage("Serienummer moet ingevuld zijn");
                RuleFor(x => x.Description).NotEmpty().WithMessage("Beschrijving moet ingevuld zijn");
                RuleFor(x => x.EstimatedValue).NotEmpty().WithMessage("Geschatte waarde moet ingevuld zijn");
                RuleFor(x => x.Year).NotEmpty().WithMessage("Bouwjaar moet ingevuld zijn")
                                    .LessThanOrEqualTo(DateTime.Now.Year).WithMessage($"Bouwjaar mag niet hoger zijn dan {DateTime.Now.Year}")
                                    .GreaterThanOrEqualTo(1900).WithMessage("Bouwjaar mag niet lager zijn dan 1900");
                RuleFor(x => x.QuoteNumber).NotEmpty().WithMessage("De ingeruilde machine moet bij een offerte horen");
                RuleFor(x => x.ImageContentType).Must(images => images != null && images.Any()).WithMessage("Er moet minstens één afbeelding gekozen zijn.");
            }
        }
    }
}
