using Xunit;
using Moq;
using ClinicManagement.Infrastructure.Services;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Infrastructure.Data;
using ClinicManagement.Infrastructure.Repositories;

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
        public async Task MarkPresentAsync_ValidSession_AddsAttendance()
        {
            var session = new Session
            {
                Id = 1,
                Status = SessionStatus.Confirmed,
                Appointment = new Appointment { Date = DateTime.Today }
            };
            _sessionRepoMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(session);
            _attendanceRepoMock.Setup(a => a.GetByPatientIdAndDateAsync("P1", DateTime.Today)).ReturnsAsync((Attendance)null);
            _attendanceRepoMock.Setup(a => a.AddAsync(It.IsAny<Attendance>())).Returns(Task.CompletedTask);

            var result = await _service.MarkPresentAsync(1, "P1", "Notes");

            Assert.True(result >= 0);
            _attendanceRepoMock.Verify(a => a.AddAsync(It.IsAny<Attendance>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task MarkPresentAsync_InvalidSession_ThrowsException()
        {
            _sessionRepoMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Session)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.MarkPresentAsync(1, "P1", "Notes"));
        }

        [Fact]
        public async Task MarkPresentAsync_AlreadyExists_ThrowsException()
        {
            var session = new Session
            {
                Id = 1,
                Status = SessionStatus.Confirmed,
                Appointment = new Appointment { Date = DateTime.Today }
            };
            _sessionRepoMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(session);
            _attendanceRepoMock.Setup(a => a.GetByPatientIdAndDateAsync("P1", DateTime.Today))
                .ReturnsAsync(new Attendance { Id = 10 });

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.MarkPresentAsync(1, "P1", "Notes"));
        }
        #endregion

        #region MarkAbsentAsync Tests
        [Fact]
        public async Task MarkAbsentAsync_ValidSession_AddsAttendance()
        {
            var session = new Session
            {
                Id = 1,
                Status = SessionStatus.Confirmed,
                Appointment = new Appointment { Date = DateTime.Today }
            };
            _sessionRepoMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(session);
            _attendanceRepoMock.Setup(a => a.GetByPatientIdAndDateAsync("P1", DateTime.Today)).ReturnsAsync((Attendance)null);
            _attendanceRepoMock.Setup(a => a.AddAsync(It.IsAny<Attendance>())).Returns(Task.CompletedTask);

            var result = await _service.MarkAbsentAsync(1, "P1", "Notes");

            Assert.True(result >= 0);
            _attendanceRepoMock.Verify(a => a.AddAsync(It.IsAny<Attendance>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task MarkAbsentAsync_InvalidSession_ThrowsException()
        {
            _sessionRepoMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Session)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.MarkAbsentAsync(1, "P1", "Notes"));
        }

        [Fact]
        public async Task MarkAbsentAsync_AlreadyExists_ThrowsException()
        {
            var session = new Session
            {
                Id = 1,
                Status = SessionStatus.Confirmed,
                Appointment = new Appointment { Date = DateTime.Today }
            };
            _sessionRepoMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(session);
            _attendanceRepoMock.Setup(a => a.GetByPatientIdAndDateAsync("P1", DateTime.Today))
                .ReturnsAsync(new Attendance { Id = 10 });

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.MarkAbsentAsync(1, "P1", "Notes"));
        }
        #endregion

        #region GetDailySummaryReportAsync Tests
        [Fact]
        public async Task GetDailySummaryReportAsync_ReturnsCorrectSummary_InMemory()
        {
            var options = new DbContextOptionsBuilder<ClinicDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var date = DateTime.Today;

            using (var context = new ClinicDbContext(options, null))
            {
                var doctor = new Doctor
                {
                    Id = "D1",
                    FirstName = "Dr",
                    LastName = "Smith",
                    IsActive = true
                };

                var patient1 = new Patient { Id = "P1", FirstName = "John", LastName = "Doe", NationID = "12345678901234", IsActive = true };
                var patient2 = new Patient { Id = "P2", FirstName = "Alice", LastName = "Smith", NationID = "23456789012345", IsActive = true };
                var patient3 = new Patient { Id = "P3", FirstName = "Bob", LastName = "Brown", NationID = "34567890123456", IsActive = true };

                context.Doctors.Add(doctor);
                context.Patients.AddRange(patient1, patient2, patient3);
                context.SaveChanges();

                var appointment1 = new Appointment { Id = 1, DoctorId = doctor.Id, PatientId = patient1.Id, Date = date, Notes = "First session" };
                var appointment2 = new Appointment { Id = 2, DoctorId = doctor.Id, PatientId = patient2.Id, Date = date, Notes = "Second session" };
                var appointment3 = new Appointment { Id = 3, DoctorId = doctor.Id, PatientId = patient3.Id, Date = date, Notes = "Third session" };

                context.Appointments.AddRange(appointment1, appointment2, appointment3);
                context.SaveChanges();

                var session1 = new Session { Id = 1, DoctorId = doctor.Id, PatientId = patient1.Id, Status = SessionStatus.Confirmed, AppointmentId = appointment1.Id, Appointment = appointment1 };
                var session2 = new Session { Id = 2, DoctorId = doctor.Id, PatientId = patient2.Id, Status = SessionStatus.Confirmed, AppointmentId = appointment2.Id, Appointment = appointment2 };
                var session3 = new Session { Id = 3, DoctorId = doctor.Id, PatientId = patient3.Id, Status = SessionStatus.Confirmed, AppointmentId = appointment3.Id, Appointment = appointment3 };

                context.Sessions.AddRange(session1, session2, session3);

                context.Attendances.AddRange(
                    new Attendance { PatientId = patient1.Id, SessionId = session1.Id, IsPresent = true },
                    new Attendance { PatientId = patient2.Id, SessionId = session2.Id, IsPresent = false },
                    new Attendance { PatientId = patient3.Id, SessionId = session3.Id, IsPresent = true }
                );

                context.SaveChanges();

                var unitOfWork = new UnitOfWork(context);
                var service = new AttendanceService(unitOfWork);

                var result = await service.GetDailySummaryReportAsync(date);

                Assert.Equal(2, result.PresentCount);
                Assert.Equal(1, result.AbsentCount);
                Assert.Equal(3, result.TotalPatients);
                Assert.Equal(date, result.Date);
            }
        }
        #endregion
    }

}
