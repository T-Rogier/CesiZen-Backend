using FluentValidation;
using CesiZen_Backend.Dtos.AuthDtos;

namespace CesiZen_Backend.Validators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).EmailAddress().WithMessage("Adresse email invalide");
            RuleFor(x => x.Password).Equal(z => z.ConfirmPassword).WithMessage("Le mot de passe ne correspond pas");
        }
    }
}
