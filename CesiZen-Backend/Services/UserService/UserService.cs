using CesiZen_Backend.Dtos.ActivityDtos;
using CesiZen_Backend.Dtos.UserDtos;
using CesiZen_Backend.Models;
using CesiZen_Backend.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_Backend.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly CesiZenDbContext _dbContext;
        private readonly ILogger<UserService> _logger;
        public UserService(CesiZenDbContext dbContext, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<FullUserResponseDto> CreateUserAsync(CreateUserRequestDto command)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(command.Password);

            User user = User.Create(command.Username, command.Email, hashedPassword, command.Role);

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return UserMapper.ToFullDto(user);
        }

        public async Task<UserListResponseDto> GetAllUsersAsync()
        {
            List<User> users = await _dbContext.Users
                .AsNoTracking()
                .ToListAsync();
            return UserMapper.ToListDto(users, users.Count);
        }

        public async Task<UserListResponseDto> GetUsersByFilterAsync(UserFilterRequestDto filter)
        {
            int pageNumber = Math.Max(1, filter.PageNumber);
            int pageSize = Math.Max(1, filter.PageSize);

            IQueryable<User> query = _dbContext.Users;

            if (!string.IsNullOrWhiteSpace(filter.Username))
                query = query.Where(a => a.Username.ToLower().Contains(filter.Username.ToLower()));

            if (!string.IsNullOrWhiteSpace(filter.Email))
                query = query.Where(a => a.Email.ToLower().Contains(filter.Email.ToLower()));

            if (filter.StartDate.HasValue)
                query = query.Where(a => a.Created >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(a => a.Created <= filter.EndDate.Value);

            if (filter.Disabled.HasValue)
                query = query.Where(a => a.Disabled == filter.Disabled.Value);

            if (filter.Role.HasValue)
            {
                query = query.Where(a => a.Role == filter.Role.Value);
            }

            int totalCount = await query.CountAsync();

            List<User> users = await query
                .AsNoTracking()
                .OrderByDescending(a => a.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return UserMapper.ToListDto(users, totalCount, pageNumber, pageSize);
        }

        public Task<UserRoleListReponseDto> GetUserRolesAsync()
        {
            IEnumerable<UserRole> userRoles = Enum.GetValues<UserRole>();
            return Task.FromResult(new UserRoleListReponseDto(userRoles));
        }

        public async Task<FullUserResponseDto?> GetUserByIdAsync(int id)
        {
            User? user = await _dbContext.Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return null;

            return UserMapper.ToFullDto(user);
        }

        public async Task UpdateUserAsync(int id, UpdateUserRequestDto command)
        {
            User? userToUpdate = await _dbContext.Users.FindAsync(id);
            if (userToUpdate == null)
                throw new ArgumentNullException($"Invalid User Id.");
            userToUpdate.Update(command.Username, command.Password, command.Role, command.Disabled);
            _dbContext.Entry(userToUpdate).Property(c => c.Updated).IsModified = true;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            User? userToDelete = await _dbContext.Users.FindAsync(id);
            if (userToDelete != null)
            {
                userToDelete.Delete();
                _dbContext.Entry(userToDelete).Property(c => c.Updated).IsModified = true;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
