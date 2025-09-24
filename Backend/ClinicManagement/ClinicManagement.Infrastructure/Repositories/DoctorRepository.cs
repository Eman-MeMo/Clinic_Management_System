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
    public class DoctorRepository : UserRepository<Doctor>,IDoctorRepository
    {
        public DoctorRepository(ClinicDbContext db) : base(db)
        {
        }
        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await db.Set<Doctor>().AsNoTracking().Include(d=> d.Specialization).ToListAsync();
        }

        public async Task<Doctor> GetByIdAsync(string id)
        {
            return await db.Set<Doctor>().AsNoTracking().Include(d => d.Specialization).FirstOrDefaultAsync(p=> p.Id == id);
        }
        public async Task<Doctor> GetByEmailAsync(string email)
        {
            return await db.Set<Doctor>().AsNoTracking().Include(d => d.Specialization).FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<IEnumerable<Doctor>> GetAllBySpecializationAsync(string specialization)
        {
            return await db.Set<Doctor>().AsNoTracking().Where(d => d.Specialization.Name == specialization).ToListAsync();
        }
    }
}
