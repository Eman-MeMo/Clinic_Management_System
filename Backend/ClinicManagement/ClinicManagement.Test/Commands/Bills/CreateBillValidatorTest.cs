using ClinicManagement.Application.Commands.Bills.CreateBill;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Bills
{
    public class CreateBillValidatorTest
    {
        private readonly CreateBillValidator _validator;

        public CreateBillValidatorTest()
        {
            _validator = new CreateBillValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Amount_Is_Zero_Or_Negative()
        {
            var model = new CreateBillCommand { Amount = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Amount must be greater than zero.");
        }

        [Fact]
        public void Should_Have_Error_When_Date_Is_Empty()
        {
            var model = new CreateBillCommand { Date = default };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Date)
                .WithErrorMessage("Date is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Date_In_Future()
        {
            var model = new CreateBillCommand { Date = DateTime.UtcNow.AddDays(1) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Date)
                .WithErrorMessage("Date cannot be in the future.");
        }


        [Fact]
        public void Should_Have_Error_When_SessionId_Not_Positive()
        {
            var model = new CreateBillCommand { SessionId = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.SessionId)
                .WithErrorMessage("Session ID must be a positive number.");
        }

        [Fact]
        public void Should_Have_Error_When_PatientId_Is_Empty()
        {
            var model = new CreateBillCommand { PatientId = null };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.PatientId)
                .WithErrorMessage("Patient ID is required.");
        }

        [Fact]
        public void Should_Pass_When_All_Fields_Valid()
        {
            var model = new CreateBillCommand
            {
                Amount = 100,
                Date = DateTime.UtcNow.AddDays(-1),
                IsPaid = true,
                SessionId = 5,
                PatientId = "P1"
            };

            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}