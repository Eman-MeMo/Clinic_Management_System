using ClinicManagement.Application.Commands.Users.LoginUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Users
{
    public class LoginUserValidatorTest
    {
        private readonly LoginUserValidator _validator;

        public LoginUserValidatorTest()
        {
            _validator = new LoginUserValidator();
        }

        [Fact]
        public void Validate_ValidCommand_ReturnsSuccess()
        {
            var command = new LoginUserCommand
            {
                Email = "user@example.com",
                Password = "secure123"
            };

            var result = _validator.Validate(command);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_EmptyEmail_ReturnsError()
        {
            var command = new LoginUserCommand
            {
                Email = "",
                Password = "secure123"
            };

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        }

        [Fact]
        public void Validate_InvalidEmail_ReturnsError()
        {
            var command = new LoginUserCommand
            {
                Email = "invalidemail",
                Password = "secure123"
            };

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        }

        [Fact]
        public void Validate_ShortPassword_ReturnsError()
        {
            var command = new LoginUserCommand
            {
                Email = "user@example.com",
                Password = "123"
            };

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Password");
        }

        [Fact]
        public void Validate_EmptyPassword_ReturnsError()
        {
            var command = new LoginUserCommand
            {
                Email = "user@example.com",
                Password = ""
            };

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Password");
        }
    }
}