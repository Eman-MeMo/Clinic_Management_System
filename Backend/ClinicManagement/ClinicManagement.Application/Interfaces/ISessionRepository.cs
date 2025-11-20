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
        Task<Session> GetByIdWithAttendanceAsync(int sessionId);
        Task<Session> GetWithAppointmentByIdAsync(int sessionId);
        Task<int> CreateSessionAsync(int appointmentId);
        Task<IEnumerable<Session>> GetSessionsByDoctor(string doctorId);
        IQueryable<Session> GetSessionsByDoctorAsAsQueryable(string doctorId);
        IQueryable<Session> GetSessionsByPatientAsQueryable(string patientId);
        Task<IEnumerable<Session>> GetSessionsByPatient(string patientId);
        Task UpdateDoctorNotesAsync(int sessionId, string notes);
        Task<bool> HasSessionForAppointmentAsync(int appointmentId);
    }
}
