using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IUserService<T> where T : AppUser
    {
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetAllAsQueryable();
        Task<T> GetByIdAsync(string id);
        Task<T> GetByEmailAsync(string email);
        Task AddAsync(T entity);
        Task Delete(T entity);
        Task Update(T entity);
    }
}
