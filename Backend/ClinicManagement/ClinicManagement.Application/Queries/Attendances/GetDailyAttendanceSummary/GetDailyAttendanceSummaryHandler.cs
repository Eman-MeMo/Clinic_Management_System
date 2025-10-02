using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Attendances.GetDailyAttendanceSummary
{
    public class GetDailyAttendanceSummaryHandler : IRequestHandler<GetDailyAttendanceSummaryQuery, AttendanceSummaryDto>
    {
        private readonly IAttendanceService _attendanceService;

        public GetDailyAttendanceSummaryHandler(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        public async Task<AttendanceSummaryDto> Handle(GetDailyAttendanceSummaryQuery request, CancellationToken cancellationToken)
        {
            return await _attendanceService.GetDailySummaryReportAsync(request.Date);
        }
    }
}
