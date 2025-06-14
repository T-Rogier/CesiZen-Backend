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
            UserRole role = Enum.Parse<UserRole>(command.Role);

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(command.Password);

            User user = User.Create(command.Username, command.Email, hashedPassword, role);

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return UserMapper.ToFullDto(user);
        }

        public async Task<UserListResponseDto> GetAllUsersAsync()
        {
            List<User> users = await _dbContext.Users
                .AsNoTracking()
                .ToListAsync();
            return UserMapper.ToListDto(users, 1, 10000, users.Count);
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
            UserRole role = Enum.Parse<UserRole>(command.Role);

            User? userToUpdate = await _dbContext.Users.FindAsync(id);
            if (userToUpdate == null)
                throw new ArgumentNullException($"Invalid User Id.");
            userToUpdate.Update(command.Username, command.Email, command.Password, role, command.Disabled);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            User? userToDelete = await _dbContext.Users.FindAsync(id);
            if (userToDelete != null)
            {
                _dbContext.Users.Remove(userToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
