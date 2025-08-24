using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManagment.Domain.Entities;
using TaskManagment.Infrastructure.UnitOfWork;
using TaskManagment.Service.Dtos;
using TaskManagment.Service.IServices;

namespace TaskManagment.Service.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaskService(IUnitOfWork uow, IMapper mapper)
        {
            _unitOfWork = uow; _mapper = mapper;
        }

        private static bool IsAdmin(ClaimsPrincipal user) => user.IsInRole("Admin");

        public async Task<TaskDto> CreateAsync(TaskCreateDto dto)
        {

            var entity = _mapper.Map<TaskItems>(dto);

            await _unitOfWork.Tasks.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();


            entity = await _unitOfWork.Tasks.GetDetailedByIdAsync(entity.Id) ?? entity;
            return _mapper.Map<TaskDto>(entity);
        }

        public async Task<IEnumerable<TaskDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.Tasks.GetAllAsync();

            return _mapper.Map<IEnumerable<TaskDto>>(entities);
        }

        public async Task<TaskDto?> GetByIdAsync(int id, ClaimsPrincipal currentUser)
        {
            var entity = await _unitOfWork.Tasks.GetDetailedByIdAsync(id);
            if (entity is null) return null;

            if (!IsAdmin(currentUser))
            {
                var uid = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (entity.AssignedUserId != uid) return null;
            }


            return _mapper.Map<TaskDto>(entity);
        }

        public async Task<TaskDto?> UpdateAsync(int id, TaskUpdateDto dto, ClaimsPrincipal currentUser)
        {
            var entity = await _unitOfWork.Tasks.GetDetailedByIdAsync(id);
            if (entity is null) return null;

            if (IsAdmin(currentUser))
            {

                _mapper.Map(dto, entity);
            }
            else
            {
                var uid = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (entity.AssignedUserId != uid) return null;
                if (!dto.Status.HasValue)
                    throw new InvalidOperationException("Only status can be updated by a regular user.");


                _mapper.Map(new TaskStatusUpdateDto(dto.Status.Value), entity);
            }

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TaskDto>(entity);
        }

        public async Task<TaskDto?> UpdateStatusAsync(int id, TaskStatusUpdateDto dto, ClaimsPrincipal currentUser)
        {
            var entity = await _unitOfWork.Tasks.GetDetailedByIdAsync(id);
            if (entity is null) return null;

            var uid = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!IsAdmin(currentUser) && entity.AssignedUserId != uid) return null;


            _mapper.Map(dto, entity);

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TaskDto>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (entity is null) return false;

            _unitOfWork.Tasks.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TaskDto>> GetMyTasksAsync(ClaimsPrincipal currentUser)
        {
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
                return Enumerable.Empty<TaskDto>();

            var tasks = await _unitOfWork.Tasks.GetByAssignedUserAsync(userId);

            return tasks.Select(t => _mapper.Map<TaskDto>(t));
        }
    }
}
