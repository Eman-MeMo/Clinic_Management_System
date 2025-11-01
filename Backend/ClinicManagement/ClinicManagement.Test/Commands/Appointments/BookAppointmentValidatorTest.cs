using ClinicManagement.Application.Commands.Appointments.BookAppointment;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Appointments
{
    public class BookAppointmentValidatorTest
    {
        private readonly BookAppointmentValidator _validator;

        public BookAppointmentValidatorTest()
        {
            _validator = new BookAppointmentValidator();
        }

        [Fact]
        public void Validator_Should_Have_Error_When_DoctorId_Is_Empty()
        {
            var model = new BookAppointmentCommand
            {
                DoctorId = "",
                PatientId = "P1",
                Date = DateTime.UtcNow.AddDays(1)
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DoctorId);
        }

        [Fact]
        public void Validator_Should_Have_Error_When_Date_Is_In_Past()
        {
            var model = new BookAppointmentCommand
            {
                DoctorId = "D1",
                PatientId = "P1",
                Date = DateTime.UtcNow.AddDays(-1)
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Date);
        }

        [Fact]
        public void Validator_Should_Pass_For_Valid_Command()
        {
            var model = new BookAppointmentCommand
            {
                DoctorId = "D1",
                PatientId = "P1",
                Date = DateTime.UtcNow.AddDays(1),
                Notes = "Normal visit"
            };

            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
