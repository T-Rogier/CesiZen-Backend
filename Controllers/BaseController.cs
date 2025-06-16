using CesiZen_Backend.Models;
using CesiZen_Backend.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen_Backend.Controllers
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        private User? _currentUser;
        private readonly ICurrentUserService _currentUserService;

        protected ApiControllerBase(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        protected async Task<User> GetCurrentUserAsync()
        {
            if (_currentUser == null)
            {
                _currentUser = await _currentUserService.GetUserAsync();
            }
            return _currentUser;
        }

        protected async Task<User?> TryGetCurrentUserAsync()
        {
            if (_currentUser == null)
            {
                _currentUser = await _currentUserService.TryGetUserAsync();
            }
            return _currentUser;
        }
    }
}
