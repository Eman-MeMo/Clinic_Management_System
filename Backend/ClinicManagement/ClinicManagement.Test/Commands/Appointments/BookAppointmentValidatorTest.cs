using ClinicManagement.Application.Commands.Appointments.BookAppointment;
using FluentValidation.TestHelper;
using System;
using Xunit;

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
                Date = DateTime.Now.AddDays(1)
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DoctorId);
        }

        [Fact]
        public void Validator_Should_Have_Error_When_PatientId_Is_Empty()
        {
            var model = new BookAppointmentCommand
            {
                DoctorId = "D1",
                PatientId = "",
                Date = DateTime.Now.AddDays(1)
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.PatientId);
        }

        [Fact]
        public void Validator_Should_Have_Error_When_Date_Is_In_Past()
        {
            var model = new BookAppointmentCommand
            {
                DoctorId = "D1",
                PatientId = "P1",
                Date = DateTime.Now.AddDays(-1)
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Date);
        }

        [Fact]
        public void Validator_Should_Have_Error_When_Notes_Exceeds_MaxLength()
        {
            var model = new BookAppointmentCommand
            {
                DoctorId = "D1",
                PatientId = "P1",
                Date = DateTime.Now.AddDays(1),
                Notes = new string('A', 501) // 501 chars
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Notes);
        }

        [Fact]
        public void Validator_Should_Pass_For_Valid_Command()
        {
            var model = new BookAppointmentCommand
            {
                DoctorId = "D1",
                PatientId = "P1",
                Date = DateTime.Now.AddDays(1),
                Notes = "Normal visit"
            };

            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
