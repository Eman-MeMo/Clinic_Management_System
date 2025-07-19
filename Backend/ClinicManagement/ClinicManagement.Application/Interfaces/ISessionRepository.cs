using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<int> CreateSessionAsync(int appointmentId);
        Task EndSession(int sessionId, SessionStatus status);
        Task<IEnumerable<Session>> GetSessionsByDoctor(string doctorId);
        IQueryable<Session> GetSessionsByDoctorAsAsQueryable(string doctorId);
        IQueryable<Session> GetSessionsByPatientAsQueryable(string patientId);
        Task<IEnumerable<Session>> GetSessionsByPatient(string patientId);
        Task AddDoctorNotes(int sessionId, string notes);
        Task<bool> HasSessionForAppointmentAsync(int appointmentId);
    }
}
