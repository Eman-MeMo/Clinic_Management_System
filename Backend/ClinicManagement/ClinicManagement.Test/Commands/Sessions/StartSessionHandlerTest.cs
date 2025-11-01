using ClinicManagement.Application.Commands.Sessions.StartSession;
using ClinicManagement.Application.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Sessions
{
    public class StartSessionHandlerTest
    {
        private readonly Mock<ISessionManagementService> _sessionServiceMock;
        private readonly StartSessionHandler _handler;

        public StartSessionHandlerTest()
        {
            _sessionServiceMock = new Mock<ISessionManagementService>();
            _handler = new StartSessionHandler(_sessionServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSessionId()
        {
            var command = new StartSessionCommand { AppointmentId = 1 };
            var expectedSessionId = 10;

            _sessionServiceMock
                .Setup(s => s.StartSessionAsync(command.AppointmentId))
                .ReturnsAsync(expectedSessionId);

            var result = await _handler.Handle(command, CancellationToken.None);

            _sessionServiceMock.Verify(s => s.StartSessionAsync(command.AppointmentId), Times.Once);
            Assert.Equal(expectedSessionId, result);
        }

        [Fact]
        public async Task Handle_NullCommand_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<System.ArgumentNullException>(() =>
                _handler.Handle(null, CancellationToken.None));
        }
    }
}