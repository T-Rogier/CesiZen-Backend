﻿using CesiZen_Backend.Dtos.UserDtos;

namespace CesiZen_Backend.Services.UserService
{
    public interface IUserService
    {
        Task<FullUserResponseDto> CreateUserAsync(CreateUserRequestDto command);
        Task<FullUserResponseDto?> GetUserByIdAsync(int id);
        Task<UserListResponseDto> GetAllUsersAsync();
        Task<UserListResponseDto> GetUsersByFilterAsync(UserFilterRequestDto filter);
        Task<UserRoleListReponseDto> GetUserRolesAsync();
        Task UpdateUserAsync(int id, UpdateUserRequestDto command);
        Task DeleteUserAsync(int id);
    }
}
