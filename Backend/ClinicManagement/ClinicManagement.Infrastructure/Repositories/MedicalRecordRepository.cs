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
    public class MedicalRecordRepository : GenericRepository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(ClinicDbContext db) : base(db) { }
        public async Task<IEnumerable<MedicalRecord>> GetByDateAsync(DateTime date)
        {
            return await db.MedicalRecords.AsNoTracking().Where(m => m.Date.Date == date.Date).ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(string patientId)
        {
            return await db.MedicalRecords.AsNoTracking().Where(m => m.PatientId == patientId).ToListAsync();
        }

        public async Task<MedicalRecord> GetLatestRecordAsync(string patientId)
        {
            return await db.MedicalRecords.AsNoTracking()
                .Where(m => m.PatientId == patientId)
                .OrderByDescending(m => m.Date)
                .FirstOrDefaultAsync();
        }
    }
}
