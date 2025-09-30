using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Validations;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Appointments.UpdateAppointment
{
    public class UpdateAppointmentCommand:IRequest<Unit>
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
    }
}
