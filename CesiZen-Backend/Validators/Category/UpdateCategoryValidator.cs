using CesiZen_Backend.Dtos.CategoryDtos;
using FluentValidation;

namespace CesiZen_Backend.Validators.Category
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequestDto>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Le nom est requis.")
                .MaximumLength(100).WithMessage("Le nom ne doit pas dépasser 100 caractères.");

            RuleFor(x => x.IconLink)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Le lien de l'icon est requis.")
                .Must(link => Uri.TryCreate(link, UriKind.Absolute, out _)).WithMessage("Le lien de l'icon n’est pas valide.")
                .MaximumLength(500).WithMessage("Le lien de l'icon ne doit pas dépasser 500 caractères.");
        }
    }
}
