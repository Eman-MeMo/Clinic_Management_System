using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Queries.Appointments.GetAppointmentsByPatient
{
    public class GetAppointmentsByPatientQuery:IRequest<IEnumerable<AppointmentDto>>
    {
        public string PatientId { get; set; }
    }
}
