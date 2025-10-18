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
    public class PatientRepository : UserRepository<Patient>,IPatientRepository
    {
        protected readonly ClinicDbContext db;
        public PatientRepository(ClinicDbContext _db):base(_db)
        {
        }
        public async Task<Patient> GetByNationalIdAsync(string nationId)
        {
            return await db.Set<Patient>().FirstOrDefaultAsync(p => p.NationID == nationId);
        }
    }
}
