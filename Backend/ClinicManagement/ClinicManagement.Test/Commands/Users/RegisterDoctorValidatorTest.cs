using ClinicManagement.Application.Commands.Users.RegisterDoctor;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Users
{
    public class RegisterDoctorValidatorTest
    {
        private readonly RegisterDoctorValidator _validator;

        public RegisterDoctorValidatorTest()
        {
            _validator = new RegisterDoctorValidator();
        }

        [Fact]
        public void Validator_ValidData_PassesValidation()
        {
            var command = new RegisterDoctorCommand
            {
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "+1234567890",
                SpecializationId = 1,
                Email = "john@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validator_MissingFields_ReturnsErrors()
        {
            var command = new RegisterDoctorCommand();

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.FirstName);
            result.ShouldHaveValidationErrorFor(x => x.LastName);
            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
            result.ShouldHaveValidationErrorFor(x => x.SpecializationId);
            result.ShouldHaveValidationErrorFor(x => x.Email);
            result.ShouldHaveValidationErrorFor(x => x.Password);
            result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
        }

        [Fact]
        public void Validator_InvalidEmail_ReturnsError()
        {
            var command = new RegisterDoctorCommand
            {
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "+1234567890",
                SpecializationId = 1,
                Email = "invalidemail",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Validator_PasswordsDoNotMatch_ReturnsError()
        {
            var command = new RegisterDoctorCommand
            {
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "+1234567890",
                SpecializationId = 1,
                Email = "john@example.com",
                Password = "Password123",
                ConfirmPassword = "Different123"
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
        }
    }
}
