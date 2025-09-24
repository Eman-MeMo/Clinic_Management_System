using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace ClinicManagement.Infrastructure.Repositories
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        public SessionRepository(ClinicDbContext db) : base(db)
        {

        }

        public async Task AddDoctorNotes(int sessionId, string notes)
        {
            var session = await db.Set<Session>().FirstOrDefaultAsync(s => s.Id == sessionId);
            if (session != null)
            {
                session.Notes = notes;
                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Session>> GetSessionsByDoctor(string doctorId)
        {
            return await db.Set<Session>().AsNoTracking()
                .Where(s => s.DoctorId == doctorId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Session>> GetSessionsByPatient(string patientId)
        {
            return await db.Set<Session>().AsNoTracking()
                .Where(s => s.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<int> CreateSessionAsync(int appointmentId)
        {
            var appointment = await db.Set<Appointment>().FindAsync(appointmentId);
            var session = new Session
            {
                AppointmentId = appointmentId,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                ActualStartTime = DateTime.Now,
                Status=SessionStatus.Scheduled,
                Notes = string.Empty
            };
            await db.Set<Session>().AddAsync(session);
            await db.SaveChangesAsync();
            return session.Id;
        }
        public async Task<bool> HasSessionForAppointmentAsync(int appointmentId)
        {
            return await db.Sessions.AsNoTracking().AnyAsync(s => s.AppointmentId == appointmentId);
        }

        public IQueryable<Session> GetSessionsByDoctorAsAsQueryable(string doctorId)
        {
            return db.Set<Session>().AsNoTracking().Where(s => s.DoctorId == doctorId);
        }

        public IQueryable<Session> GetSessionsByPatientAsQueryable(string patientId)
        {
            return db.Set<Session>().AsNoTracking().Where(s => s.PatientId == patientId);
        }
    }
}
