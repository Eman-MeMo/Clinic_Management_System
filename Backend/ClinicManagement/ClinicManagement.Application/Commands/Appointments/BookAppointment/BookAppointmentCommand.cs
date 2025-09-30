using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Validations;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Appointments.BookAppointment
{
    public class BookAppointmentCommand:IRequest<int>
    {
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
    }
}
