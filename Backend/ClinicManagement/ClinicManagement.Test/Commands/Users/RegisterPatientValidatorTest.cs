using ClinicManagement.Application.Commands.Users.RegisterPatient;
using ClinicManagement.Domain.Enums;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Users
{
    public class RegisterPatientValidatorTest
    {
        private readonly RegisterPatientValidator _validator;

        public RegisterPatientValidatorTest()
        {
            _validator = new RegisterPatientValidator();
        }

        [Fact]
        public void Validator_ValidData_PassesValidation()
        {
            var command = new RegisterPatientCommand
            {
                FirstName = "John",
                Lastname = "Doe",
                PhoneNumber = "+1234567890",
                Email = "john@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123",
                NationID = "12345678901234",
                DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 5, 10)),
                gender = Gender.Male
            };

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validator_MissingRequiredFields_ReturnsErrors()
        {
            var command = new RegisterPatientCommand();

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.FirstName);
            result.ShouldHaveValidationErrorFor(x => x.Lastname);
            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
            result.ShouldHaveValidationErrorFor(x => x.Email);
            result.ShouldHaveValidationErrorFor(x => x.Password);
            result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
            result.ShouldHaveValidationErrorFor(x => x.NationID);
            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
        }

        [Fact]
        public void Validator_InvalidPhoneNumber_ReturnsError()
        {
            var command = new RegisterPatientCommand { PhoneNumber = "123" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        }

        [Fact]
        public void Validator_InvalidEmail_ReturnsError()
        {
            var command = new RegisterPatientCommand { Email = "invalid-email" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Validator_PasswordsDoNotMatch_ReturnsError()
        {
            var command = new RegisterPatientCommand
            {
                Password = "Password123",
                ConfirmPassword = "WrongPassword"
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
        }

        [Fact]
        public void Validator_InvalidNationID_ReturnsError()
        {
            var command = new RegisterPatientCommand { NationID = "123" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.NationID);
        }

        [Fact]
        public void Validator_FutureDateOfBirth_ReturnsError()
        {
            var command = new RegisterPatientCommand
            {
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
        }
    }
}