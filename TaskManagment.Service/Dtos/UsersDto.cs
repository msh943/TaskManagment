using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagment.Service.Dtos
{
    public record CreateUserDto(string Email, string Password, string FullName, string Role);
    public record UpdateUserDto(string? Email, string? FullName, string? Role);
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
