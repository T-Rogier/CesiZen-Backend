using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Models;
using FluentValidation;

namespace CesiZen_Backend.Validators.Activity
{
    public class CreateActivityValidator : AbstractValidator<CreateActivityRequestDto>
    {
        public CreateActivityValidator()
        {
            RuleFor(x => x.Title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Le titre est requis.")
                .MaximumLength(200).WithMessage("Le titre ne doit pas dépasser 200 caractères.");

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La description est requise.")
                .MaximumLength(2000).WithMessage("La description ne doit pas dépasser 2000 caractères.");

            RuleFor(x => x.Content)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Le contenu est requis.")
                .MaximumLength(20000).WithMessage("Le contenu ne doit pas dépasser 20000 caractères.");

            RuleFor(x => x.ThumbnailImageLink)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Le lien de l’image miniature est requis.")
                .Must(link => Uri.TryCreate(link, UriKind.Absolute, out _)).WithMessage("Le lien de l’image n’est pas valide.")
                .MaximumLength(1000).WithMessage("Le lien de l'image ne doit pas dépasser 1000 caractères.");

            RuleFor(x => x.EstimatedDuration)
                .GreaterThan(TimeSpan.Zero).WithMessage("La durée estimée doit être supérieure à 0.");

            RuleFor(x => x.Categories)
                .NotNull().WithMessage("La liste des catégories est requise.");

            RuleForEach(x => x.Categories)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Une catégorie ne peut pas être vide.")
                .MaximumLength(100).WithMessage("Une catégorie ne doit pas dépasser 100 caractères.");

            RuleFor(x => x.Type)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Le type ne peut pas être vide.")
                .Must(type => Enum.TryParse<ActivityType>(type, out _)).WithMessage("Le type doit être 'Classique', 'Écriture', 'Jeu' ou 'Playlist'.");
        }
    }
}
