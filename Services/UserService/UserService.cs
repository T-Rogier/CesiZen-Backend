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

        public async Task<UserDto> CreateUserAsync(CreateUserDto command)
        {
            UserRole role = Enum.Parse<UserRole>(command.Role);

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(command.Password);

            User user = User.Create(command.Username, command.Email, hashedPassword, role);

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return UserMapper.ToDto(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Select(u => UserMapper.ToDto(u))
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            User? user = await _dbContext.Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return null;

            return UserMapper.ToDto(user);
        }

        public async Task UpdateUserAsync(int id, UpdateUserDto command)
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
