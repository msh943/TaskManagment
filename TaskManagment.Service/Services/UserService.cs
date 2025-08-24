using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagment.Domain.Identity;
using TaskManagment.Service.Dtos;
using TaskManagment.Service.IServices;

namespace TaskManagment.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<AppUser> um, RoleManager<IdentityRole> rm, IMapper mapper)
        {
            _userManager = um; _roleManager = rm; _mapper = mapper;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            var user = new AppUser { Email = dto.Email, UserName = dto.Email, FullName = dto.FullName };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

            if (!await _roleManager.RoleExistsAsync(dto.Role)) throw new InvalidOperationException($"Role '{dto.Role}' does not exist.");
            await _userManager.AddToRoleAsync(user, dto.Role);

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = dto.Role;
            return userDto;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return false;
            return (await _userManager.DeleteAsync(user)).Succeeded;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var dtos = _mapper.Map<List<UserDto>>(users);
            for (int i = 0; i < users.Count; i++)
            {
                var roles = await _userManager.GetRolesAsync(users[i]);
                dtos[i].Role = roles.FirstOrDefault() ?? "";
            }
            return dtos;
        }

        public async Task<UserDto?> GetUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return null;
            var dto = _mapper.Map<UserDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            dto.Role = roles.FirstOrDefault() ?? "";
            return dto;
        }

        public async Task<UserDto?> UpdateUserAsync(string id, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Email)) { user.Email = dto.Email; user.UserName = dto.Email; }
            if (!string.IsNullOrWhiteSpace(dto.FullName)) user.FullName = dto.FullName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                var current = await _userManager.GetRolesAsync(user);
                if (current.Any()) await _userManager.RemoveFromRolesAsync(user, current);
                if (!await _roleManager.RoleExistsAsync(dto.Role)) throw new InvalidOperationException($"Role '{dto.Role}' does not exist.");
                await _userManager.AddToRoleAsync(user, dto.Role);
            }

            var userDto = _mapper.Map<UserDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Role = roles.FirstOrDefault() ?? "";
            return userDto;
        }
    }
}
