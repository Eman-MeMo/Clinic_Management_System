using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Bills.CreateBill
{
    public class CreateBillValidator:AbstractValidator<CreateBillCommand>
    {
        public CreateBillValidator() {
            RuleFor(b => b.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");

            RuleFor(b => b.Date)
                .NotEmpty().WithMessage("Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Date cannot be in the future.");

            RuleFor(b => b.IsPaid)
                .NotNull().WithMessage("Payment status is required.");

            RuleFor(b => b.SessionId)
                .GreaterThan(0).WithMessage("Session ID must be a positive number.");

            RuleFor(b => b.PatientId)
                .NotEmpty().WithMessage("Patient ID is required.");
        }
    }
}
