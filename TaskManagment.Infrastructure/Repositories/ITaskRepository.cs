using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagment.Domain.Entities;

namespace TaskManagment.Infrastructure.Repositories
{
    public interface ITaskRepository : IRepository<TaskItems>
    {
        Task<TaskItems?> GetDetailedByIdAsync(int id);
        Task<IEnumerable<TaskItems>> GetByAssignedUserAsync(string userId);
    }
}
