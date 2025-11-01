using ClinicManagement.Application.Commands.Attendances.MarkPresent;
using ClinicManagement.Application.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Attendances
{
    public class MarkPresentHandlerTest
    {
        private readonly Mock<IAttendanceService> _attendanceServiceMock;
        private readonly MarkPresentHandler _handler;

        public MarkPresentHandlerTest()
        {
            _attendanceServiceMock = new Mock<IAttendanceService>();
            _handler = new MarkPresentHandler(_attendanceServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsAttendanceId()
        {
            var command = new MarkPresentCommand
            {
                SessionId = 1,
                PatientId = "P1",
                Notes = "Attended successfully"
            };

            _attendanceServiceMock
                .Setup(a => a.MarkPresentAsync(command.SessionId, command.PatientId, command.Notes))
                .ReturnsAsync(10);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(10, result);
            _attendanceServiceMock.Verify(a => a.MarkPresentAsync(command.SessionId, command.PatientId, command.Notes), Times.Once);
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_CallsAttendanceServiceOnce()
        {
            var command = new MarkPresentCommand
            {
                SessionId = 5,
                PatientId = "P2",
                Notes = "On time"
            };

            _attendanceServiceMock
                .Setup(a => a.MarkPresentAsync(command.SessionId, command.PatientId, command.Notes))
                .ReturnsAsync(15);

            await _handler.Handle(command, CancellationToken.None);

            _attendanceServiceMock.Verify(a =>
                a.MarkPresentAsync(command.SessionId, command.PatientId, command.Notes),
                Times.Once);
        }
    }
}