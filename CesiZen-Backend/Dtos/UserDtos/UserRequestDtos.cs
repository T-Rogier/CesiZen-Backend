using CesiZen_Backend.Common.Converter;
using CesiZen_Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_Backend.Dtos.UserDtos
{
    public record CreateUserRequestDto(
        string Username,
        string Email,
        string Password,
        string ConfirmPassword,
        UserRole Role
    );

    public record UpdateUserRequestDto(
        string? Username,
        string? Password,
        string? ConfirmPassword,
        UserRole Role
    );

    public record UserFilterRequestDto(
        string? Query,
        bool? Disabled,
        [ModelBinder(BinderType = typeof(DisplayNameEnumModelBinder<UserRole>))]
        UserRole? Role,
        DateTimeOffset? StartDate,
        DateTimeOffset? EndDate,
        int PageNumber = 1,
        int PageSize = 10
    );
}
