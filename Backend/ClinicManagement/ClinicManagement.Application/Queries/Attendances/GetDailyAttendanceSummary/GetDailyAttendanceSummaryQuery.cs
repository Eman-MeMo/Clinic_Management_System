using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Attendances.GetDailyAttendanceSummary
{
    public class GetDailyAttendanceSummaryQuery: IRequest<AttendanceSummaryDto>
    {
        public DateTime Date { get; set; }
    }
}
