using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IAttendanceRepository:IGenericRepository<Attendance>
    {
        Task<Attendance> GetBySessionIdAsync(int sessionId);
        Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date);
        Task<Attendance> GetByPatientIdAndDateAsync(string patientId, DateTime date);
        Task<IEnumerable<Patient>> GetAbsentPatientsByDateAsync(DateTime date);
        Task<AttendanceSummaryDto> GetDailySummaryReportAsync(DateTime date);
    }
}
