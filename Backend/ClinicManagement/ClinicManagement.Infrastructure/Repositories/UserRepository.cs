using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Repositories
{
    public class UserRepository<T> : IUserRepository<T> where T : AppUser
    {
        protected readonly ClinicDbContext db;
        public UserRepository(ClinicDbContext _db)
        {
            db = _db;
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await db.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(string id)
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
        public async Task<T> GetByEmailAsync(string email)
        {
            return await db.Set<T>().FirstOrDefaultAsync(p => p.Email == email);
        }
        public async Task<T> DeactivateUserAsync(string id)
        {
            var user = await db.Set<T>().FindAsync(id);
            if (user != null)
            {
                user.IsActive = false;
                db.Set<T>().Update(user);
            }
            return user;
        }
    }
}
