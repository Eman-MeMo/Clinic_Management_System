using ClinicManagement.Application.Commands.Users.DeactivateUser;
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
    public class DeactivateUserHandlerTest
    {
        private readonly Mock<IUserDeactivationStrategy> strategyMock;
        private readonly List<IUserDeactivationStrategy> strategies;
        private readonly DeactivateUserHandler handler;

        public DeactivateUserHandlerTest()
        {
            strategyMock = new Mock<IUserDeactivationStrategy>();
            strategies = new List<IUserDeactivationStrategy> { strategyMock.Object };
            handler = new DeactivateUserHandler(strategies);
        }

        [Fact]
        public async Task Handle_Should_CallDeactivate_When_StrategyExists()
        {
            var command = new DeactivateUserCommand { UserId = "user123", UserType = "Doctor" };
            strategyMock.Setup(s => s.CanHandle("Doctor")).Returns(true);

            await handler.Handle(command, CancellationToken.None);

            strategyMock.Verify(s => s.DeactivateAsync("user123"), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_When_NoStrategyFound()
        {
            var command = new DeactivateUserCommand { UserId = "admin1", UserType = "Admin" };
            strategyMock.Setup(s => s.CanHandle(It.IsAny<string>())).Returns(false);

            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_Return_UnitValue_When_Successful()
        {
            var command = new DeactivateUserCommand { UserId = "pateint123", UserType = "Patient" };
            strategyMock.Setup(s => s.CanHandle("Patient")).Returns(true);
            strategyMock.Setup(s => s.DeactivateAsync("pateint123")).Returns(Task.CompletedTask);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Equal(Unit.Value, result);
        }
    }
}