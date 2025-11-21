using ClinicManagement.Application.Commands.WorkSchedules.CreateWorkSchedule;
using FluentValidation.TestHelper;
using System;
using Xunit;

namespace ClinicManagement.Test.Commands.WorkSchedules
{
    public class CreateWorkScheduleValidatorTest
    {
        private readonly CreateWorkScheduleValidator _validator;

        public CreateWorkScheduleValidatorTest()
        {
            _validator = new CreateWorkScheduleValidator();
        }

        [Fact]
        public void Validate_ValidCommand_ReturnsSuccess()
        {
            var command = new CreateWorkScheduleCommand
            {
                DoctorId = "D1",
                DayOfWeek = DayOfWeek.Monday,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(3),
                IsAvailable = true
            };

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_MissingDoctorId_ReturnsError()
        {
            var command = new CreateWorkScheduleCommand
            {
                DoctorId = "",
                DayOfWeek = DayOfWeek.Monday,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(3),
                IsAvailable = true
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.DoctorId)
                  .WithErrorMessage("Doctor ID is required.");
        }

        [Fact]
        public void Validate_StartTimeAfterEndTime_ReturnsError()
        {
            var command = new CreateWorkScheduleCommand
            {
                DoctorId = "D1",
                DayOfWeek = DayOfWeek.Tuesday,
                StartTime = DateTime.Now.AddHours(5),
                EndTime = DateTime.Now.AddHours(2),
                IsAvailable = true
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StartTime)
                  .WithErrorMessage("Start time must be earlier than end time.");
            result.ShouldHaveValidationErrorFor(x => x.EndTime)
                  .WithErrorMessage("End time must be later than start time.");
        }

        [Fact]
        public void Validate_InvalidDayOfWeek_ReturnsError()
        {
            var command = new CreateWorkScheduleCommand
            {
                DoctorId = "D1",
                DayOfWeek = (DayOfWeek)10,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(3),
                IsAvailable = true
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.DayOfWeek)
                  .WithErrorMessage("Invalid Day of week!");
        }

        [Fact]
        public void Validate_StartTimeNotSet_ReturnsError()
        {
            var command = new CreateWorkScheduleCommand
            {
                DoctorId = "D1",
                DayOfWeek = DayOfWeek.Wednesday,
                StartTime = default,
                EndTime = DateTime.Now.AddHours(3),
                IsAvailable = true
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.StartTime)
                  .WithErrorMessage("Start time is required.");
        }

        [Fact]
        public void Validate_EndTimeNotSet_ReturnsError()
        {
            var command = new CreateWorkScheduleCommand
            {
                DoctorId = "D1",
                DayOfWeek = DayOfWeek.Wednesday,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = default,
                IsAvailable = true
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.EndTime)
                  .WithErrorMessage("End time is required.");
        }
    }
}
