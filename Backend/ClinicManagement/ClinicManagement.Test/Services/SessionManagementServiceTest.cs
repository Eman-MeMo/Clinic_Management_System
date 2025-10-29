using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Infrastructure.Repositories;
using ClinicManagement.Infrastructure.Services;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagement.Test.Services
{
    public class SessionManagementServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ISessionRepository> _sessionRepoMock;
        private readonly Mock<IAppointmentRepository> _appointmentRepoMock;
        private readonly ISessionManagementService _service;

        public SessionManagementServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sessionRepoMock = new Mock<ISessionRepository>();
            _appointmentRepoMock = new Mock<IAppointmentRepository>();

            _unitOfWorkMock.Setup(u => u.SessionRepository).Returns(_sessionRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.AppointmentRepository).Returns(_appointmentRepoMock.Object);

            _service = new SessionManagementService(_unitOfWorkMock.Object);
        }
        #region EndSessionAsync Tests
        [Fact]
        public async Task EndSessionAsync_SessionNotFound_ThrowsException()
        {
            _sessionRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Session)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.EndSessionAsync(1, SessionStatus.Confirmed));
        }

        [Fact]
        public async Task EndSessionAsync_InvalidStatus_ThrowsException()
        {
            var session = new Session { Id = 1, Status = SessionStatus.Confirmed };
            _sessionRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(session);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.EndSessionAsync(1, SessionStatus.Cancelled));
        }

        [Fact]
        public async Task EndSessionAsync_ValidScheduledSession_UpdatesStatusAndAppointment()
        {
            var appointment = new Appointment { Id = 1, Status = AppointmentStatus.Confirmed };
            var session = new Session { Id = 1, Status = SessionStatus.Scheduled, Appointment = appointment };

            _sessionRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(session);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            await _service.EndSessionAsync(1, SessionStatus.Confirmed);

            Assert.Equal(SessionStatus.Confirmed, session.Status);
            Assert.Equal(AppointmentStatus.Confirmed, session.Appointment.Status);
            Assert.NotNull(session.ActualEndTime);

            _sessionRepoMock.Verify(r => r.Update(session), Times.Once);
            _appointmentRepoMock.Verify(r => r.Update(session.Appointment), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
        #endregion

        #region StartSessionAsync Tests
        [Fact]
        public async Task StartSessionAsync_AppointmentNotFound_ThrowsException()
        {
            _appointmentRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Appointment)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.StartSessionAsync(1));
        }

        [Fact]
        public async Task StartSessionAsync_SessionAlreadyExists_ThrowsException()
        {
            var appointment = new Appointment { Id = 1, Date = DateTime.Now, Status = AppointmentStatus.Confirmed };
            _appointmentRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);
            _sessionRepoMock.Setup(r => r.HasSessionForAppointmentAsync(1)).ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.StartSessionAsync(1));
        }

        [Fact]
        public async Task StartSessionAsync_BeforeAllowedTime_ThrowsException()
        {
            var appointment = new Appointment { Id = 1, Date = DateTime.Now.AddMinutes(20), Status = AppointmentStatus.Confirmed };
            _appointmentRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);
            _sessionRepoMock.Setup(r => r.HasSessionForAppointmentAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.StartSessionAsync(1));
        }

        [Fact]
        public async Task StartSessionAsync_AppointmentNotConfirmed_ThrowsException()
        {
            var appointment = new Appointment { Id = 1, Date = DateTime.Now.AddMinutes(-5), Status = AppointmentStatus.Scheduled };
            _appointmentRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);
            _sessionRepoMock.Setup(r => r.HasSessionForAppointmentAsync(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.StartSessionAsync(1));
        }

        [Fact]
        public async Task StartSessionAsync_ValidAppointment_CreatesSession()
        {
            var appointment = new Appointment { Id = 1, Date = DateTime.Now.AddMinutes(-5), Status = AppointmentStatus.Confirmed };
            _appointmentRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);
            _sessionRepoMock.Setup(r => r.HasSessionForAppointmentAsync(1)).ReturnsAsync(false);
            _sessionRepoMock.Setup(r => r.CreateSessionAsync(1)).ReturnsAsync(100);

            var sessionId = await _service.StartSessionAsync(1);

            Assert.Equal(100, sessionId);
            _sessionRepoMock.Verify(r => r.CreateSessionAsync(1), Times.Once);
        }
        #endregion
    }
}
