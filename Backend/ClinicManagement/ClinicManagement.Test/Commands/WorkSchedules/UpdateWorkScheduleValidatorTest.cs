using ClinicManagement.Application.Commands.WorkSchedules.UpdateWorkSchedule;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.WorkSchedules
{
    public class UpdateWorkScheduleValidatorTest
    {
        private readonly UpdateWorkScheduleValidator validator;

        public UpdateWorkScheduleValidatorTest()
        {
            validator = new UpdateWorkScheduleValidator();
        }

        [Fact]
        public void Should_HaveError_When_StartTimeIsEmpty()
        {
            var model = new UpdateWorkScheduleCommand
            {
                StartTime = default,
                EndTime = DateTime.Now.AddHours(1),
                DoctorId = "D1",
                DayOfWeek = DayOfWeek.Monday,
                IsAvailable = true,
                Id = 1
            };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.StartTime);
        }

        [Fact]
        public void Should_HaveError_When_EndTimeIsEmpty()
        {
            var model = new UpdateWorkScheduleCommand
            {
                StartTime = DateTime.Now,
                EndTime = default,
                DoctorId = "D1",
                DayOfWeek = DayOfWeek.Monday,
                IsAvailable = true,
                Id = 1
            };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.EndTime);
        }

        [Fact]
        public void Should_HaveError_When_EndTimeBeforeStartTime()
        {
            var model = new UpdateWorkScheduleCommand
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(-1),
                DoctorId = "D1",
                DayOfWeek = DayOfWeek.Monday,
                IsAvailable = true,
                Id = 1
            };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.EndTime);
        }

        [Fact]
        public void Should_HaveError_When_DoctorIdIsZero()
        {
            var model = new UpdateWorkScheduleCommand
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                DoctorId = null,
                DayOfWeek = DayOfWeek.Monday,
                IsAvailable = true,
                Id = 1
            };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DoctorId);
        }

        [Fact]
        public void Should_HaveError_When_IdIsNotGreaterThanZero()
        {
            var model = new UpdateWorkScheduleCommand
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                DoctorId = "D1",
                DayOfWeek = DayOfWeek.Monday,
                IsAvailable = true,
                Id = 0
            };
            var result = validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_NotHaveError_When_ModelIsValid()
        {
            var model = new UpdateWorkScheduleCommand
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(2),
                DoctorId = "D1",
                DayOfWeek = DayOfWeek.Tuesday,
                IsAvailable = true,
                Id = 10
            };
            var result = validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}