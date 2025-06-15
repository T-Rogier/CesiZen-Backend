using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CesiZen_Backend.Services.UserService
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CesiZenDbContext _dbContext;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, CesiZenDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public async Task<User> GetUserAsync()
        {
            ClaimsPrincipal? userPrincipal = _httpContextAccessor.HttpContext?.User;
            if (userPrincipal is null)
                throw new UnauthorizedAccessException("Aucun utilisateur connecté");

            Claim? idClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim is null || !int.TryParse(idClaim.Value, out int userId))
                throw new UnauthorizedAccessException("Identifiant utilisateur invalide dans le token");

            User? user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                throw new UnauthorizedAccessException("Utilisateur introuvable");

            return user;
        }

        public async Task<User?> TryGetUserAsync()
        {
            ClaimsPrincipal? userPrincipal = _httpContextAccessor.HttpContext?.User;
            if (userPrincipal is null)
                return null;

            Claim? idClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim is null || !int.TryParse(idClaim.Value, out int userId))
                throw new UnauthorizedAccessException("Identifiant utilisateur invalide dans le token");

            User? user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                throw new UnauthorizedAccessException("Utilisateur introuvable");

            return user;
        }
    }
}
