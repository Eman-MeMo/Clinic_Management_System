using ClinicManagement.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Appointments.UpdateAppointmentStatus
{
    public class UpdateAppointmentStatusCommand:IRequest<Unit>
    {
        public int AppointmentId { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}
