using CesiZen_Backend.Dtos.AuthDtos;
using FluentValidation;

namespace CesiZen_Backend.Validators.Auth
{
    public class RegisterValidator  : AbstractValidator<RegisterRequestDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Username)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Le nom d'utilisateur ne peut pas être vide.")
                .Length(3, 50).WithMessage("Le nom d'utilisateur doit contenir entre 3 et 50 caractères.");
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("L'email ne peut pas être vide.")
                .EmailAddress().WithMessage("Adresse email invalide.");
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Le mot de passe ne peut pas être vide.")
                .MinimumLength(8).WithMessage("Le mot de passe doit contenir au moins 8 caractères.")
                .MaximumLength(16).WithMessage("Le mot de passe ne doit pas dépasser 16 caractères.")
                .Matches(@"[A-Z]+").WithMessage("Le mot de passe doit contenir au moins une lettre majuscule.")
                .Matches(@"[a-z]+").WithMessage("Le mot de passe doit contenir au moins une lettre minuscule.")
                .Matches(@"[0-9]+").WithMessage("Le mot de passe doit contenir au moins un chiffre.")
                .Matches(@"[\!\?\*\.]+").WithMessage("Le mot de passe doit contenir au moins un caractère spécial parmi (! ? * .).")
                .Equal(z => z.ConfirmPassword).WithMessage("Le mot de passe ne correspond pas");
        }
    }
}
