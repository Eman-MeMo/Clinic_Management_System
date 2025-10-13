using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Services
{
    public class UserService<T> : IUserService<T> where T : AppUser
    {
        private readonly IUserRepository<T> userRepository;
        private readonly IUnitOfWork unitOfWork;
        public UserService(IUserRepository<T> _userRepository, IUnitOfWork _unitOfWork)
        {
            userRepository = _userRepository;
            unitOfWork = _unitOfWork;
        }
        public async Task AddAsync(T entity)
        {
             await userRepository.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task Delete(T entity)
        {
            userRepository.Delete(entity);
            await unitOfWork.SaveChangesAsync();
        }

        public IQueryable<T> GetAllAsQueryable()
        {
            return userRepository.GetAllAsQueryable();
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return userRepository.GetAllAsync();
        }

        public async Task<T> GetByEmailAsync(string email)
        {
             return await userRepository.GetByEmailAsync(email);
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await userRepository.GetByIdAsync(id);
        }

        public async Task Update(T entity)
        {
             userRepository.Update(entity);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
