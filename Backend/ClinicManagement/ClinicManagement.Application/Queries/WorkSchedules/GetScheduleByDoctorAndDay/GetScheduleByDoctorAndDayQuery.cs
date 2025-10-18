using ClinicManagement.Domain.DTOs.WorkScheduleDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.WorkSchedules.GetScheduleByDoctorAndDay
{
    public class GetScheduleByDoctorAndDayQuery:IRequest<WorkScheduleDto>
    {
        public string DoctorId { get; set; }
        public DayOfWeek Day { get; set; }
    }
}
