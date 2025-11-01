using ClinicManagement.Application.Commands.Users.RegisterPatient;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Users
{
    public class RegisterPatientHandlerTest
    {
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly RegisterPatientHandler _handler;

        public RegisterPatientHandlerTest()
        {
            _mockAccountService = new Mock<IAccountService>();
            _handler = new RegisterPatientHandler(_mockAccountService.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_CallsRegisterPatientAsync()
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
                gender = Gender.Male,
                DateOfBirth = new DateOnly(1995, 5, 10)
            };

            var expectedResult = new object();
            _mockAccountService.Setup(x => x.RegisterPatientAsync(
                command.FirstName,
                command.Lastname,
                command.PhoneNumber,
                command.Email,
                command.Password,
                command.ConfirmPassword,
                command.NationID,
                command.gender,
                command.DateOfBirth)).ReturnsAsync(expectedResult);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(expectedResult, result);
            _mockAccountService.Verify(x => x.RegisterPatientAsync(
                command.FirstName,
                command.Lastname,
                command.PhoneNumber,
                command.Email,
                command.Password,
                command.ConfirmPassword,
                command.NationID,
                command.gender,
                command.DateOfBirth), Times.Once);
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
        }
    }
}