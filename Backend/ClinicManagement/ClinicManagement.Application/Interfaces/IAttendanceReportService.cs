using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IAttendanceReportService
    {
        Task<AttendanceSummaryDto> GetDailySummaryReportAsync(DateTime date);
    }
}
