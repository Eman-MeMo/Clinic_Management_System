using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Services
{
    public class AttendanceService:IAttendanceService
    {
        public readonly IUnitOfWork unitOfWork;
        public AttendanceService(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public async Task<int> MarkPresentAsync(int sessionId, string patientId, string? notes)
        {
            var session = await unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if (session == null || session.Status != SessionStatus.Confirmed)
                throw new InvalidOperationException("Session not valid for attendance.");

            var existing = await unitOfWork.AttendanceRepository
                .GetByPatientIdAndDateAsync(patientId, session.Appointment.Date);
            if (existing != null)
                throw new InvalidOperationException("Attendance already recorded.");

            var attendance = new Attendance
            {
                SessionId = sessionId,
                PatientId = patientId,
                IsPresent = true,
                Notes = notes
            };

            await unitOfWork.AttendanceRepository.AddAsync(attendance);
            await unitOfWork.SaveChangesAsync();
            return attendance.Id;
        }

        public async Task<int> MarkAbsentAsync(int sessionId, string patientId, string? notes)
        {
            var session = await unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if (session == null || session.Status != SessionStatus.Confirmed)
                throw new InvalidOperationException("Session not valid for attendance.");

            var existing = await unitOfWork.AttendanceRepository
                .GetByPatientIdAndDateAsync(patientId, session.Appointment.Date);
            if (existing != null)
                throw new InvalidOperationException("Attendance already recorded for this patient on this date.");

            var attendance = new Attendance
            {
                SessionId = sessionId,
                PatientId = patientId,
                IsPresent = false,
                Notes = notes
            };

            await unitOfWork.AttendanceRepository.AddAsync(attendance);
            await unitOfWork.SaveChangesAsync();
            return attendance.Id;
        }

        public async Task<AttendanceSummaryDto> GetDailySummaryReportAsync(DateTime date)
        {
            var attendancesOnDate = await unitOfWork.AttendanceRepository.GetAllAsQueryable()
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
