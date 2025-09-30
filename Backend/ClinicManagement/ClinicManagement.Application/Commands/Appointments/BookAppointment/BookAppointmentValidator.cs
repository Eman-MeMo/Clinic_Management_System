using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Appointments.BookAppointment
{
    public class BookAppointmentValidator : AbstractValidator<BookAppointmentCommand>
    {
        public BookAppointmentValidator()
        {
            RuleFor(x => x.DoctorId)
                .NotEmpty().WithMessage("DoctorId is required.");

            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("PatientId is required.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required.")
                .GreaterThan(DateTime.Now).WithMessage("Appointment date must be in the future.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes must be less than 500 characters.");
        }
    }
}
