using Moq;
using ClinicManagement.Infrastructure.Services;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using ClinicManagement.Infrastructure.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Test.Services
{
    public class AttendanceServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ISessionRepository> _sessionRepoMock;
        private readonly Mock<IAttendanceRepository> _attendanceRepoMock;
        private readonly AttendanceService _service;

        public AttendanceServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sessionRepoMock = new Mock<ISessionRepository>();
            _attendanceRepoMock = new Mock<IAttendanceRepository>();

            _unitOfWorkMock.Setup(u => u.SessionRepository).Returns(_sessionRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.AttendanceRepository).Returns(_attendanceRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _service = new AttendanceService(_unitOfWorkMock.Object);
        }

        #region MarkPresentAsync Tests

        [Fact]
        public async Task MarkPresentAsync_ValidSession_UpdatesExistingAttendance()
        {
            var session = new Session { Id = 1, Status = SessionStatus.Confirmed };
            var attendance = new Attendance { Id = 10, SessionId = 1, IsPresent = false, PatientId = "P1" };

            _sessionRepoMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(session);
            _attendanceRepoMock.Setup(a => a.GetBySessionIdAsync(1)).ReturnsAsync(attendance);
            _attendanceRepoMock.Setup(a => a.Update(It.IsAny<Attendance>())).Verifiable();

            await _service.MarkPresentAsync(1, "P1", "Note");

            _attendanceRepoMock.Verify(a => a.Update(It.Is<Attendance>(att => att.IsPresent)), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task MarkPresentAsync_InvalidSession_ThrowsException()
        {
            _sessionRepoMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Session)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.MarkPresentAsync(1, "P1", "Note"));
        }

        [Fact]
        public async Task MarkPresentAsync_MissingAttendance_ThrowsException()
        {
            var session = new Session { Id = 1, Status = SessionStatus.Confirmed };
            _sessionRepoMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(session);
            _attendanceRepoMock.Setup(a => a.GetBySessionIdAsync(1)).ReturnsAsync((Attendance)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.MarkPresentAsync(1, "P1", "Note"));
        }

        #endregion

        #region MarkAbsentAsync Tests

        [Fact]
        public async Task MarkAbsentAsync_ValidSession_UpdatesExistingAttendance()
        {
            var session = new Session { Id = 1, Status = SessionStatus.Confirmed };
            var attendance = new Attendance { Id = 10, SessionId = 1, IsPresent = true, PatientId = "P1" };

            _sessionRepoMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(session);
            _attendanceRepoMock.Setup(a => a.GetBySessionIdAsync(1)).ReturnsAsync(attendance);
            _attendanceRepoMock.Setup(a => a.Update(It.IsAny<Attendance>())).Verifiable();

            await _service.MarkAbsentAsync(1, "P1", "Note");

            _attendanceRepoMock.Verify(a => a.Update(It.Is<Attendance>(att => !att.IsPresent)), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task MarkAbsentAsync_InvalidSession_ThrowsException()
        {
            _sessionRepoMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Session)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.MarkAbsentAsync(1, "P1", "Note"));
        }

        [Fact]
        public async Task MarkAbsentAsync_MissingAttendance_ThrowsException()
        {
            var session = new Session { Id = 1, Status = SessionStatus.Confirmed };
            _sessionRepoMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(session);
            _attendanceRepoMock.Setup(a => a.GetBySessionIdAsync(1)).ReturnsAsync((Attendance)null);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.MarkAbsentAsync(1, "P1", "Note"));
        }

        #endregion
        #region GetDailySummaryReportAsync Tests    
        [Theory]
        [InlineData(2, 0)] // all present
        [InlineData(0, 2)] // all absent
        [InlineData(1, 1)] // mixed
        [InlineData(0, 0)] // none
        public async Task GetDailySummaryReportAsync_ReturnsCorrectCounts(int present, int absent)
        {
            var options = new DbContextOptionsBuilder<ClinicDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var loggerMock = new Mock<ILogger<ClinicDbContext>>();
            var date = DateTime.Today;

            using (var context = new ClinicDbContext(options, loggerMock.Object))
            {
                for (int i = 0; i < present; i++)
                {
                    var appointment = new Appointment { Date = date, PatientId = $"P{i + 1}", DoctorId = "D1" , Notes="Test"};
                    var session = new Session { Appointment = appointment, PatientId = appointment.PatientId , DoctorId = "D1" };
                    context.Attendances.Add(new Attendance { IsPresent = true, Session = session, PatientId = appointment.PatientId });
                }

                for (int i = 0; i < absent; i++)
                {
                    var appointment = new Appointment { Date = date, PatientId = $"A{i + 1}", DoctorId = "D1" , Notes = "Test" };
                    var session = new Session { Appointment = appointment, PatientId = appointment.PatientId , DoctorId = "D1" };
                    context.Attendances.Add(new Attendance { IsPresent = false, Session = session, PatientId = appointment.PatientId });
                }

                await context.SaveChangesAsync();

                var unitOfWork = new UnitOfWork(context);
                var service = new AttendanceService(unitOfWork);

                var summary = await service.GetDailySummaryReportAsync(date);

                Assert.Equal(present + absent, summary.TotalPatients);
                Assert.Equal(present, summary.PresentCount);
                Assert.Equal(absent, summary.AbsentCount);
                Assert.Equal(date, summary.Date);
            }
        }
        #endregion
    }
}
