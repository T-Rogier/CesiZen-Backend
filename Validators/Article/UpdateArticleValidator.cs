using CesiZen_Backend.Dtos.ArticleDtos;
using FluentValidation;

namespace CesiZen_Backend.Validators.Article
{
    public class UpdateArticleValidator : AbstractValidator<UpdateArticleRequestDto>
    {
        public UpdateArticleValidator()
        {
            RuleFor(x => x.Title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Le titre est requis.")
                .MaximumLength(200).WithMessage("Le titre ne doit pas dépasser 200 caractères.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Le contenu est requis.");
        }
    }
}
