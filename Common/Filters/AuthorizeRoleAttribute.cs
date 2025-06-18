using CesiZen_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CesiZen_Backend.Common.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly UserRole[] _allowedRoles;

        public AuthorizeRoleAttribute(params UserRole[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            ClaimsPrincipal user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            string? roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;

            if (roleClaim == null || !Enum.TryParse(roleClaim, out UserRole userRole))
            {
                context.Result = new ForbidResult();
                return;
            }

            if (!_allowedRoles.Contains(userRole))
            {
                context.Result = new ForbidResult();
                return;
            }

            await Task.CompletedTask;
        }
    }
}
