using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagment.Service.Dtos;
using TaskManagment.Service.IServices;

namespace TaskManagment.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : BaseController
    {
        private readonly IUserService _svc;
        public UsersController(IUserService svc) { _svc = svc; }

        [HttpGet] public async Task<ActionResult<IEnumerable<UserDto>>> GetAll() => Ok(await _svc.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> Get(string id)
            => (await _svc.GetUserAsync(id)) is { } dto ? Ok(dto) : NotFound();

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create(CreateUserDto dto)
        {
            var created = await _svc.CreateUserAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<UserDto>> Update(string id, UpdateUserDto dto)
            => (await _svc.UpdateUserAsync(id, dto)) is { } updated ? Ok(updated) : NotFound();

        [HttpPost("{id}")]
        public async Task<ActionResult> Delete(string id)
            => (await _svc.DeleteUserAsync(id)) ? NoContent() : NotFound();
    }
}
