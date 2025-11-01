using ClinicManagement.Application.Commands.Users.RegisterDoctor;
using ClinicManagement.Application.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Users
{
    public class RegisterDoctorHandlerTest
    {
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly RegisterDoctorHandler _handler;

        public RegisterDoctorHandlerTest()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _handler = new RegisterDoctorHandler(_accountServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_CallsRegisterDoctorAsync()
        {
            var request = new RegisterDoctorCommand
            {
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "+1234567890",
                Email = "john@example.com",
                Password = "Password123",
                ConfirmPassword = "Password123",
                SpecializationId = 1
            };

            _accountServiceMock
                .Setup(x => x.RegisterDoctorAsync(
                    request.FirstName,
                    request.LastName,
                    request.PhoneNumber,
                    request.Email,
                    request.Password,
                    request.ConfirmPassword,
                    request.SpecializationId))
                .ReturnsAsync("Success");

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.Equal("Success", result);
            _accountServiceMock.Verify(x => x.RegisterDoctorAsync(
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.Email,
                request.Password,
                request.ConfirmPassword,
                request.SpecializationId), Times.Once);
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
        }
    }
}