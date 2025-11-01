using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Payments.CreatePayment
{
    public class CreatePaymentValidator:AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentValidator() {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.Date)
            .LessThanOrEqualTo(_ => DateTime.UtcNow)
            .WithMessage("Date cannot be in the future.");


            RuleFor(x => x.BillId)
                .GreaterThan(0).WithMessage("Bill ID must be a positive integer.");

            RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Invalid payment method selected.");



        }
    }
}
