using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagment.Service.Dtos;
using TaskManagment.Service.IServices;

namespace TaskManagment.Api.Controllers
{

    public class TasksController : BaseController
    {
        private readonly ITaskService _svc;
        public TasksController(ITaskService svc) { _svc = svc; }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetAll() => Ok(await _svc.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskDto>> GetById(int id)
            => (await _svc.GetByIdAsync(id, User)) is { } dto ? Ok(dto) : NotFound();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TaskDto>> Create(TaskCreateDto dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPost("{id:int}")]
        public async Task<ActionResult<TaskDto>> Update(int id, TaskUpdateDto dto)
            => (await _svc.UpdateAsync(id, dto, User)) is { } updated ? Ok(updated) : NotFound();

        [HttpPost("{id:int}")]
        public async Task<ActionResult<TaskDto>> UpdateStatus(int id, TaskStatusUpdateDto dto)
            => (await _svc.UpdateStatusAsync(id, dto, User)) is { } updated ? Ok(updated) : NotFound();

        [HttpPost("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
            => (await _svc.DeleteAsync(id)) ? NoContent() : NotFound();

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> MyTasks() =>
            Ok(await _svc.GetMyTasksAsync(User));
    }
}
