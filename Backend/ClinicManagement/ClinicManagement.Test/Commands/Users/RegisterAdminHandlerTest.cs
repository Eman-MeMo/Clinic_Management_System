using ClinicManagement.Application.Commands.Users.RegisterAdmin;
using ClinicManagement.Application.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Users
{
    public class RegisterAdminHandlerTest
    {
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly RegisterAdminHandler _handler;

        public RegisterAdminHandlerTest()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _handler = new RegisterAdminHandler(_accountServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_CallsRegisterAdminAsync()
        {
            var command = new RegisterAdminCommand
            {
                FirstName = "Eman",
                LastName = "Ramadan",
                PhoneNumber = "01012345678",
                Email = "eman@example.com",
                Password = "Pass123!",
                ConfirmPassword = "Pass123!"
            };

            var expectedResponse = new { Success = true };
            _accountServiceMock
                .Setup(s => s.RegisterAdminAsync(
                    command.FirstName,
                    command.LastName,
                    command.PhoneNumber,
                    command.Email,
                    command.Password,
                    command.ConfirmPassword))
                .ReturnsAsync(expectedResponse);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(expectedResponse, result);
            _accountServiceMock.Verify(s => s.RegisterAdminAsync(
                command.FirstName,
                command.LastName,
                command.PhoneNumber,
                command.Email,
                command.Password,
                command.ConfirmPassword), Times.Once);
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(null, CancellationToken.None));
        }
    }
}