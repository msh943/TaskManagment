using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskManagment.Domain.Entities;
using TaskManagment.Infrastructure.Data;

namespace TaskManagment.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _db;
        public Repository(AppDbContext ctx) { _context = ctx; _db = _context.Set<T>(); }

        public async Task AddAsync(T e) => await _db.AddAsync(e);
        public async Task<IEnumerable<T>> GetAllAsync() => await _db.AsNoTracking().ToListAsync();
        public async Task<T?> GetByIdAsync(object id) => await _db.FindAsync(id);
        public void Remove(T e) => _db.Remove(e);
        public void Update(T e) => _db.Update(e);
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> p)
            => await _db.AsNoTracking().Where(p).ToListAsync();
    }
}
