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
                SessionId = 5,
                PatientId = "P1"
            };

            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}