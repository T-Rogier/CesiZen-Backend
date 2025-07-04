﻿using FluentValidation;
using CesiZen_Backend.Dtos.AuthDtos;

namespace CesiZen_Backend.Validators.Auth
{
    public class LoginValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("L'email ne peux pas être vide")
                .EmailAddress().WithMessage("Adresse email invalide");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Le mot de passe ne peut pas être vide.");
        }
    }
}
