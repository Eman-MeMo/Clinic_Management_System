using ClinicManagement.Application.Commands.Users.RegisterAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Users
{
    public class RegisterAdminValidatorTest
    {
        private readonly RegisterAdminValidator _validator;

        public RegisterAdminValidatorTest()
        {
            _validator = new RegisterAdminValidator();
        }

        [Fact]
        public void Validate_ValidCommand_ReturnsSuccess()
        {
            var command = new RegisterAdminCommand
            {
                FirstName = "Eman",
                LastName = "Ramadan",
                PhoneNumber = "+201012345678",
                Email = "eman@example.com",
                Password = "Pass123!",
                ConfirmPassword = "Pass123!"
            };

            var result = _validator.Validate(command);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_MissingFields_ReturnsFailure()
        {
            var command = new RegisterAdminCommand();

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
            Assert.Contains(result.Errors, e => e.PropertyName == "LastName");
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
            Assert.Contains(result.Errors, e => e.PropertyName == "Password");
            Assert.Contains(result.Errors, e => e.PropertyName == "ConfirmPassword");
            Assert.Contains(result.Errors, e => e.PropertyName == "PhoneNumber");
        }

        [Fact]
        public void Validate_PasswordsDoNotMatch_ReturnsFailure()
        {
            var command = new RegisterAdminCommand
            {
                FirstName = "Eman",
                LastName = "Ramadan",
                PhoneNumber = "+201012345678",
                Email = "eman@example.com",
                Password = "Pass123!",
                ConfirmPassword = "Different123!"
            };

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Passwords do not match"));
        }

        [Fact]
        public void Validate_InvalidPhoneFormat_ReturnsFailure()
        {
            var command = new RegisterAdminCommand
            {
                FirstName = "Eman",
                LastName = "Ramadan",
                PhoneNumber = "12345", 
                Email = "eman@example.com",
                Password = "Pass123!",
                ConfirmPassword = "Pass123!"
            };

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "PhoneNumber");
        }
    }
}
