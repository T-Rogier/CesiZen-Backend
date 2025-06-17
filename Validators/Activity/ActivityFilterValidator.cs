using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Models;
using FluentValidation;

namespace CesiZen_Backend.Validators.Activity
{
    public class ActivityFilterValidator : AbstractValidator<ActivityFilterRequestDto>
    {
        public ActivityFilterValidator()
        {
            RuleFor(x => x.Type)
                .Must(type => Enum.TryParse<ActivityType>(type, out _)).WithMessage("Le type doit être 'Classique', 'Écriture', 'Jeu' ou 'Playlist'.");
        }
    }
}
