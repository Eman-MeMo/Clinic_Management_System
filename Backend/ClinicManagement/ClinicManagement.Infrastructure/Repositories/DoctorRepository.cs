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
            return await db.Set<Doctor>().Include(d=> d.Specialization).ToListAsync();
        }

        public async Task<Doctor> GetByIdAsync(string id)
        {
            return await db.Set<Doctor>().Include(d => d.Specialization).FirstOrDefaultAsync(p=> p.Id == id);
        }
        public async Task<Doctor> GetByEmailAsync(string email)
        {
            return await db.Set<Doctor>().Include(d => d.Specialization).FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<IEnumerable<Doctor>> GetAllBySpecializationAsync(string specialization)
        {
            return await db.Set<Doctor>().Where(d => d.Specialization.Name == specialization).ToListAsync();
        }
        public async Task<IEnumerable<Doctor>> GetAvailableDoctorsAtAsync(DateTime targetTime)
        {
            // get all doctors who are available and do not have a session at that exact time
            var doctors = await db.Doctors
                .Where(d => d.IsAvaible && !db.Sessions
                    .Any(s => s.DoctorId == d.Id && s.Date == targetTime))
                .ToListAsync();

            return doctors;
        }
    }
}
