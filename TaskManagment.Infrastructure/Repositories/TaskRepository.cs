using Microsoft.EntityFrameworkCore;
using TaskManagment.Domain.Entities;
using TaskManagment.Infrastructure.Data;

namespace TaskManagment.Infrastructure.Repositories
{
    public class TaskRepository : Repository<TaskItems>, ITaskRepository
    {
        public TaskRepository(AppDbContext context) : base(context) { }

        public async Task<TaskItems?> GetDetailedByIdAsync(int id)
            => await _context.Tasks.Include(t => t.AssignedUser).FirstOrDefaultAsync(t => t.Id == id);

        public async Task<IEnumerable<TaskItems>> GetByAssignedUserAsync(string userId)
            => await _context.Tasks.Where(t => t.AssignedUserId == userId).ToListAsync();
    }
}
