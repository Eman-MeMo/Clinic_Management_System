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
        public async Task CreateMedicalRecoredAsync(string? Notes, string Diagnosis,int prescriptionId)
        {
            var prescription = await db.Prescriptions
                .Include(p => p.Session)
                .ThenInclude(s => s.Appointment)
                .SingleOrDefaultAsync(p => p.Id == prescriptionId);

            if (prescription == null || prescription.Session == null || prescription.Session.Appointment == null)
                throw new Exception("Prescription or related session/appointment not found.");

            var medicalRecord = new MedicalRecord
            {
                PatientId=prescription.Session.PatientId,
                Date = prescription.Session.Appointment.Date.Date,
                Notes = Notes,
                Diagnosis = Diagnosis
            };
            await db.MedicalRecords.AddAsync(medicalRecord);
            await db.SaveChangesAsync();
        }

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
