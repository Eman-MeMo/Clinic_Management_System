using ClinicManagement.Application.Commands.Payments.CreatePayment;
using ClinicManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Payments
{
    public class CreatePaymentValidatorTest
    {
        private readonly CreatePaymentValidator _validator;

        public CreatePaymentValidatorTest()
        {
            _validator = new CreatePaymentValidator();
        }

        [Fact]
        public void Validate_ValidCommand_ReturnsSuccess()
        {
            var command = new CreatePaymentCommand
            {
                Amount = 200,
                Date = DateTime.UtcNow,
                BillId = 1,
                PaymentMethod = PaymentMethod.Cash
            };

            var result = _validator.Validate(command);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_AmountLessThanOrEqualZero_ReturnsError()
        {
            var command = new CreatePaymentCommand
            {
                Amount = 0,
                Date = DateTime.UtcNow,
                BillId = 1,
                PaymentMethod = PaymentMethod.Cash
            };

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "Amount must be greater than zero.");
        }

        [Fact]
        public void Validate_DateInFuture_ReturnsError()
        {
            var command = new CreatePaymentCommand
            {
                Amount = 100,
                Date = DateTime.UtcNow.AddDays(1),
                BillId = 1,
                PaymentMethod = PaymentMethod.Cash
            };

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "Date cannot be in the future.");
        }

        [Fact]
        public void Validate_BillIdLessThanOrEqualZero_ReturnsError()
        {
            var command = new CreatePaymentCommand
            {
                Amount = 100,
                Date = DateTime.UtcNow,
                BillId = 0,
                PaymentMethod = PaymentMethod.Cash
            };

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "Bill ID must be a positive integer.");
        }
    }
}