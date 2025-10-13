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
        public async Task<IEnumerable<Doctor>> GetAllBySpecializationAsync(int specializationID)
        {
            return await db.Set<Doctor>().AsNoTracking().Where(d => d.SpecializationId == specializationID).ToListAsync();
        }
    }
}
