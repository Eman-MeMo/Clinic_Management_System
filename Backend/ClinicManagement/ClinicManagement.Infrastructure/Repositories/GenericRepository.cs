using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly ClinicDbContext db;

        public GenericRepository(ClinicDbContext _db)
        {
            db = _db;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await db.Set<T>().AsNoTracking().ToListAsync();
        }
        public IQueryable<T> GetAllAsQueryable()
        {
            return db.Set<T>().AsQueryable();
        }
        public async Task<T> GetByIdAsync(int id)
        {
            return await db.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await db.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            db.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            db.Set<T>().Remove(entity);
        }
    }
}
