using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagment.Service.Dtos
{
    public record LoginRequestDto(string Email, string Password);
    public record LoginResponseDto(string Token, string UserId, string Email, string Role);

}
