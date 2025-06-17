using FluentValidation;
using CesiZen_Backend.Dtos.UserDtos;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Validators.User
{
    public class UserFilterValidator : AbstractValidator<UserFilterRequestDto>
    {
        public UserFilterValidator()
        {
            RuleFor(x => x.Role)
                .Must(role => Enum.TryParse<UserRole>(role, out _)).WithMessage("Le rôle doit être 'Administrateur' ou 'Utilisateur'.");
        }
    }
}
