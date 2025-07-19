using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IUserRepository<T> where T : AppUser
    {
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetAllAsQueryable();
        Task<T> GetByIdAsync(string id);
        Task<T> GetByEmailAsync(string email);
        Task<T> DeactivateUserAsync(string id);
        Task AddAsync(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}
