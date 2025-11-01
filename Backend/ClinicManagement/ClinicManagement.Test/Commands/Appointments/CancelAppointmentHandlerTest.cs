using AutoMapper;
using ClinicManagement.Application.Commands.Appointments.CancelAppointment;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using MediatR;
using Moq;


namespace ClinicManagement.Test.Commands.Appointments
{
    public class CancelAppointmentHandlerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
        private readonly Mock<ISessionRepository> _sessionRepoMock;
        private readonly CancelAppointmentHandler _handler;

        public CancelAppointmentHandlerTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _appointmentRepoMock = new Mock<IAppointmentRepository>();
            _sessionRepoMock = new Mock<ISessionRepository>();

            _unitOfWorkMock.Setup(u => u.AppointmentRepository).Returns(_appointmentRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SessionRepository).Returns(_sessionRepoMock.Object);

            _handler = new CancelAppointmentHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_AppointmentNotFound_ThrowsKeyNotFoundException()
        {
            var command = new CancelAppointmentCommand { Id = 10 };
            _appointmentRepoMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync((Appointment)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_HasSession_ThrowsInvalidOperationException()
        {
            var command = new CancelAppointmentCommand { Id = 5 };
            var appointment = new Appointment { Id = 5, Status = AppointmentStatus.Scheduled };

            _appointmentRepoMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync(appointment);
            _sessionRepoMock.Setup(s => s.HasSessionForAppointmentAsync(command.Id)).ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ValidAppointment_CancelsSuccessfully()
        {
            var command = new CancelAppointmentCommand { Id = 2 };
            var appointment = new Appointment { Id = 2, Status = AppointmentStatus.Scheduled };

            _appointmentRepoMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync(appointment);
            _sessionRepoMock.Setup(s => s.HasSessionForAppointmentAsync(command.Id)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
            _appointmentRepoMock.Verify(r => r.Update(It.IsAny<Appointment>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Equal(Unit.Value, result);
        }
    }
}
