using CesiZen_Backend.Models;

namespace CesiZen_Backend.Services.UserService
{
    public interface ICurrentUserService
    {
        Task<User> GetUserAsync();
    }
}
