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
    public class PrescriptionRepository : GenericRepository<Prescription>, IPrescriptionRepository
    {
        public PrescriptionRepository(ClinicDbContext db) : base(db) { }
   
        public async Task<IEnumerable<Prescription>> GetByDoctorIdAsync(string doctorId)
        {
            return await db.Prescriptions.AsNoTracking().Include(p=>p.Session).Where(p=>p.Session.DoctorId== doctorId).ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetByPatientIdAsync(string patientId)
        {
            return await db.Prescriptions.AsNoTracking().Include(p => p.Session).Where(p => p.Session.PatientId == patientId).ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetBySessionIdAsync(int sessionId)
        {
            return await db.Prescriptions.AsNoTracking().Where(p=>p.SessionId==sessionId).ToListAsync();
        }
    }
}
