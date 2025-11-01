using ClinicManagement.Application.Commands.Users.LoginUser;
using ClinicManagement.Application.Interfaces;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Users
{
    public class LoginUserHandlerTest
    {
        private readonly Mock<IAccountService> accountServiceMock;
        private readonly LoginUserHandler handler;

        public LoginUserHandlerTest()
        {
            accountServiceMock = new Mock<IAccountService>();
            handler = new LoginUserHandler(accountServiceMock.Object);
        }

        [Fact]
        public async Task Handle_Should_CallLoginAsync_WhenRequestIsValid()
        {
            var command = new LoginUserCommand
            {
                Email = "test@example.com",
                Password = "password123"
            };

            await handler.Handle(command, CancellationToken.None);

            accountServiceMock.Verify(a => a.LoginAsync("test@example.com", "password123"), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_UnitValue_When_Successful()
        {
            var command = new LoginUserCommand
            {
                Email = "user@test.com",
                Password = "abc123"
            };

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Equal(Unit.Value, result);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentNullException_WhenRequestIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(null, CancellationToken.None));
        }
    }
}