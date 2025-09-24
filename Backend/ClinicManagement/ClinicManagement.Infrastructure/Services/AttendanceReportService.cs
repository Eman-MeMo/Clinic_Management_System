using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Services
{
    public class AttendanceReportService:IAttendanceReportService
    {
        public readonly IUnitOfWork unitOfWork;
        public AttendanceReportService(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
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
