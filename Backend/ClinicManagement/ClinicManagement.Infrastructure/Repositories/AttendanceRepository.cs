using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Application.Interfaces;

namespace ClinicManagement.Infrastructure.Repositories
{
    public class AttendanceRepository : GenericRepository<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(ClinicDbContext db) : base(db)
        {
        }

        public async Task<IEnumerable<Patient>> GetAbsentPatientsByDateAsync(DateTime date)
        {
            var attendedPatientIds = await db.Attendances.AsNoTracking()
                .Include(a => a.Session)
                .ThenInclude(s => s.Appointment)
                .Where(a => a.Session.Appointment.Date.Date == date.Date)
                .Select(a => a.PatientId)
                .Distinct()
                .ToListAsync();

            var absentPatients = await db.Patients
                .Where(p => !attendedPatientIds.Contains(p.Id))
                .ToListAsync();

            return absentPatients;
        }
        public async Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date)
        {
            return await db.Attendances.AsNoTracking()
                .Include(a => a.Session)
                .ThenInclude(s => s.Appointment)
                .Where(a => a.Session.Appointment.Date == date)
                .ToListAsync();

        }

        public async Task<Attendance> GetByPatientIdAndDateAsync(string patientId, DateTime date)
        {
            return await db.Attendances.AsNoTracking()
                .Include(a => a.Session)
                .ThenInclude(s => s.Appointment)
                .FirstOrDefaultAsync(a =>
                    a.PatientId == patientId &&
                    a.Session.Appointment.Date.Date == date.Date);
        }

        public async Task<Attendance> GetBySessionIdAsync(int sessionId)
        {
            return await db.Attendances.AsNoTracking().FirstOrDefaultAsync(a => a.SessionId == sessionId);
        }


        public async Task<AttendanceSummaryDto> GetDailySummaryReportAsync(DateTime date)
        {
            var attendancesOnDate = await db.Attendances.AsNoTracking()
                .Include(a => a.Session)
                .ThenInclude(s => s.Appointment)
                .Where(a => a.Session.Appointment.Date.Date == date.Date)
                .ToListAsync();

            var presentCount = attendancesOnDate.Count(a => a.IsPresent);
            var absentCount = attendancesOnDate.Count(a => !a.IsPresent);
            var totalPatients = attendancesOnDate.Count;

            var summary = new AttendanceSummaryDto
            {
                Date = date,
                TotalPatients = totalPatients,
                PresentCount = presentCount,
                AbsentCount = absentCount
            };
            return summary;
        }
    }
}
