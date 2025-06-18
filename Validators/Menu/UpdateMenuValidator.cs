using CesiZen_Backend.Dtos.MenuDtos;
using FluentValidation;

namespace CesiZen_Backend.Validators.Menu
{
    public class UpdateMenuValidator : AbstractValidator<UpdateMenuRequestDto>
    {
        public UpdateMenuValidator()
        {
            RuleFor(x => x.Title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Le titre est requis.")
                .MaximumLength(200).WithMessage("Le titre ne doit pas dépasser 200 caractères.");
        }
    }
}
