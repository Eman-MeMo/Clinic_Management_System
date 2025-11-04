using ClinicManagement.Domain.DTOs.WorkScheduleDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.WorkSchedules.GetWeeklySchedule
{
    public class GetWeeklyScheduleQuery:IRequest<IEnumerable<WorkScheduleDto>>
    {
        public string DoctorId { get; set; }
    }
}
