using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagment.Infrastructure.Data;
using TaskManagment.Infrastructure.Repositories;

namespace TaskManagment.Infrastructure.UnitOfWork
{
    public class UnitOfWork  : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public ITaskRepository Tasks { get; }

        public UnitOfWork(AppDbContext context, ITaskRepository tasks)
        {
            _context = context;
            Tasks = tasks;
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
