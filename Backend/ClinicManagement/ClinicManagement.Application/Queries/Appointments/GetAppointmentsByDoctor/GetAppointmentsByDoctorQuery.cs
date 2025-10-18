using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Appointments.GetAppointmentsByDoctor
{
    public class GetAppointmentsByDoctorQuery : IRequest<IEnumerable<AppointmentDto>>
    {
        public string DoctorId { get; set; }
    }
}
