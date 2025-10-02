using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Interfaces
{
    public interface IAttendanceService
    {
        Task<int> MarkPresentAsync(int sessionId, string patientId, string? notes);
        Task<int> MarkAbsentAsync(int sessionId, string patientId, string? notes);
        Task<AttendanceSummaryDto> GetDailySummaryReportAsync(DateTime date);
    }
}
