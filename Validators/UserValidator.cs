using FluentValidation;
using CesiZen_Backend.Dtos.UserDtos;

namespace CesiZen_Backend.Validators
{
    public class UserValidator : AbstractValidator<CreateUserDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email).EmailAddress().WithMessage("Adresse email invalide");
            RuleFor(x => x.Username)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Le nom d'utilisateur ne peut pas être vide")
                .MinimumLength(3).WithMessage("Le nom d'utilisateur doit comporter au moins 3 caractères")
                .MaximumLength(50).WithMessage("Le nom d'utilisateur ne peut pas dépasser 50 caractères");
            RuleFor(x => x.Password).Equal(z => z.ConfirmPassword).WithMessage("Le mot de passe ne correspond pas");
        }
    }
}
