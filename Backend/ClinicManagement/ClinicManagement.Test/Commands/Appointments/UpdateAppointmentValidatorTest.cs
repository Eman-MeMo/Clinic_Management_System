using ClinicManagement.Application.Commands.Appointments.UpdateAppointment;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Appointments
{
    public class UpdateAppointmentValidatorTest
    {
        private readonly UpdateAppointmentValidator _validator;

        public UpdateAppointmentValidatorTest()
        {
            _validator = new UpdateAppointmentValidator();
        }

        [Fact]
        public void Should_Have_Error_When_DoctorId_Is_Empty()
        {
            var command = new UpdateAppointmentCommand
            {
                Id = 1,
                DoctorId = "",
                PatientId = "P1",
                Date = DateTime.Now.AddDays(1)
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.DoctorId);
        }

        [Fact]
        public void Should_Have_Error_When_PatientId_Is_Empty()
        {
            var command = new UpdateAppointmentCommand
            {
                Id = 1,
                DoctorId = "D1",
                PatientId = "",
                Date = DateTime.Now.AddDays(1)
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.PatientId);
        }

        [Fact]
        public void Should_Have_Error_When_Date_Is_Past()
        {
            var command = new UpdateAppointmentCommand
            {
                Id = 1,
                DoctorId = "D1",
                PatientId = "P1",
                Date = DateTime.Now.AddDays(-1)
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Date);
        }

        [Fact]
        public void Should_Have_Error_When_Notes_Too_Long()
        {
            var command = new UpdateAppointmentCommand
            {
                Id = 1,
                DoctorId = "D1",
                PatientId = "P1",
                Date = DateTime.Now.AddDays(1),
                Notes = new string('a', 600)
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Notes);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Valid()
        {
            var command = new UpdateAppointmentCommand
            {
                Id = 1,
                DoctorId = "D1",
                PatientId = "P1",
                Date = DateTime.Now.AddDays(2),
                Notes = "Follow-up visit"
            };

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
