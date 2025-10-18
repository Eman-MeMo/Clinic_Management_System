using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.WorkSchedules.CreateWorkSchedule
{
    public class CreateWorkScheduleValidator:AbstractValidator<CreateWorkScheduleCommand>
    {
        public CreateWorkScheduleValidator() {
            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required.")
                .LessThan(x => x.EndTime).WithMessage("Start time must be earlier than end time.");
            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End time is required.")
                .GreaterThan(x => x.StartTime).WithMessage("End time must be later than start time.");
            RuleFor(x => x.DoctorId)
                .NotEmpty().WithMessage("Doctor ID is required.");
            RuleFor(x => x.DayOfWeek)
                .IsInEnum().WithMessage("Invalid Day of week!");
            RuleFor(x => x.IsAvailable)
                .NotNull().WithMessage("IsAvailable is required.");
        }
    }
}
