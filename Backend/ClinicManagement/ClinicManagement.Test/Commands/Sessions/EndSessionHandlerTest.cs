using ClinicManagement.Application.Commands.Sessions.EndSession;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Enums;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Sessions
{
    public class EndSessionHandlerTest
    {
        private readonly Mock<ISessionManagementService> _sessionServiceMock;
        private readonly EndSessionHandler _handler;

        public EndSessionHandlerTest()
        {
            _sessionServiceMock = new Mock<ISessionManagementService>();
            _handler = new EndSessionHandler(_sessionServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_CallsEndSessionAsync()
        {
            var command = new EndSessionCommand
            {
                SessionId = 1,
                Status = SessionStatus.Confirmed
            };

            _sessionServiceMock
                .Setup(s => s.EndSessionAsync(command.SessionId, command.Status))
                .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            _sessionServiceMock.Verify(s => s.EndSessionAsync(command.SessionId, command.Status), Times.Once);
            Assert.Equal(Unit.Value, result);
        }

        [Fact]
        public async Task Handle_NullCommand_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<System.ArgumentNullException>(() =>
                _handler.Handle(null, CancellationToken.None));
        }
    }
}