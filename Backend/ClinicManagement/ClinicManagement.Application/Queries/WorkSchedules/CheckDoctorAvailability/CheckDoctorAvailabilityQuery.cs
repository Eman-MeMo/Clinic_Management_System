using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.WorkSchedules.CheckDoctorAvailability
{
    public class CheckDoctorAvailabilityQuery:IRequest<bool>
    {
        public string DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
    }
}
