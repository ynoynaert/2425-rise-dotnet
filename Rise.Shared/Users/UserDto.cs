using FluentValidation;
using Rise.Shared.Locations;

namespace Rise.Shared.Users;
public static class UserDto
{
    public class Index
    {
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string PhoneNumber { get; set; }
        public LocationDto.Index? Location { get; set; }
    }

    public class Create 
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? PasswordConfirmation { get; set; }
        public string? PhoneNumber { get; set; }
		    public int LocationId { get; set; }
		    public class Validator : AbstractValidator<Create>
		    {
			      public Validator()
			      {
                RuleFor(x => x.Name)
                            .NotEmpty().WithMessage("Naam moet ingevuld zijn");

                RuleFor(x => x.Email)
                            .NotEmpty().WithMessage("Email moet ingevuld zijn")
                            .Matches(@"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$").WithMessage("Gelieve een geldig emailadres in te geven");

                RuleFor(x => x.PhoneNumber)
                            .NotEmpty().WithMessage("Telefoonnummer moet ingevuld zijn")
                            .Matches(@"^\+31\d{9}$").WithMessage("Telefoonnummer moet beginnen met +31 en gevolgd worden door 9 cijfers");

                        RuleFor(x => x.Password).NotEmpty().WithMessage("Wachtwoord moet ingevuld zijn")
                  .MinimumLength(8).WithMessage("Wachtwoord moet minstens 8 karakters bevatten")
                  .Matches(@"[A-Z]").WithMessage("Wachtwoord moet minstens 1 hoofdletter bevatten")
                        .Matches(@"\d").WithMessage("Wachtwoord moet minstens 1 cijfer bevatten");

                RuleFor(x => x.PasswordConfirmation)
                            .NotEmpty().WithMessage("Wachtwoord moet ingevuld zijn")
                            .Equal(x => x.Password).WithMessage("Wachtwoorden komen niet overeen");

                RuleFor(x => x.LocationId)
                  .NotEmpty().WithMessage("Locatie moet ingevuld zijn");
			      }
		    }
    }
}