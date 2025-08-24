using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManagment.Service.Dtos;

namespace TaskManagment.Service.IServices
{
    public interface ITaskService
    {
        Task<TaskDto> CreateAsync(TaskCreateDto dto);
        Task<TaskDto?> GetByIdAsync(int id, ClaimsPrincipal currentUser);
        Task<IEnumerable<TaskDto>> GetAllAsync();
        Task<TaskDto?> UpdateAsync(int id, TaskUpdateDto dto, ClaimsPrincipal currentUser);
        Task<TaskDto?> UpdateStatusAsync(int id, TaskStatusUpdateDto dto, ClaimsPrincipal currentUser);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<TaskDto>> GetMyTasksAsync(ClaimsPrincipal currentUser);
    }
}
