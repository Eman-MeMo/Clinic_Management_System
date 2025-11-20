using ClinicManagement.Application.Commands.Attendances.MarkAbsent;
using ClinicManagement.Application.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Attendances
{
    public class MarkAbsentHandlerTest
    {
        private readonly Mock<IAttendanceService> _attendanceServiceMock;
        private readonly MarkAbsentHandler _handler;

        public MarkAbsentHandlerTest()
        {
            _attendanceServiceMock = new Mock<IAttendanceService>();
            _handler = new MarkAbsentHandler(_attendanceServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenSessionIdIsLessThan1()
        {
            var command = new MarkAbsentCommand
            {
                SessionId = 0,
                PatientId = "P1",
                Notes = "Absent"
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Session ID cannot be less than 1.", ex.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnResult_WhenValidRequest()
        {
            var command = new MarkAbsentCommand
            {
                SessionId = 2,
                PatientId = "P1",
                Notes = "Absent"
            };

            _attendanceServiceMock
                .Setup(s => s.MarkAbsentAsync(command.SessionId, command.PatientId, command.Notes))
                .ReturnsAsync(1);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(1, result);
            _attendanceServiceMock.Verify(s => s.MarkAbsentAsync(command.SessionId, command.PatientId, command.Notes), Times.Once);
        }
    }
}
