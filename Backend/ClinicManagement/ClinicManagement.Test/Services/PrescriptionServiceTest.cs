using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.DTOs.PrescriptionDTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagement.Test.Services
{
    public class PrescriptionServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMedicalRecordService> _medicalRecordServiceMock;
        private readonly Mock<ISessionRepository> _sessionRepoMock;
        private readonly Mock<IPrescriptionRepository> _prescriptionRepoMock;
        private readonly PrescriptionService _service;

        public PrescriptionServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _medicalRecordServiceMock = new Mock<IMedicalRecordService>();
            _sessionRepoMock = new Mock<ISessionRepository>();
            _prescriptionRepoMock = new Mock<IPrescriptionRepository>();

            _unitOfWorkMock.Setup(u => u.SessionRepository).Returns(_sessionRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.PrescriptionRepository).Returns(_prescriptionRepoMock.Object);

            _service = new PrescriptionService(_unitOfWorkMock.Object, _mapperMock.Object, _medicalRecordServiceMock.Object);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_SessionNotFound_ThrowsException()
        {
            _sessionRepoMock.Setup(r => r.GetByIdWithAttendanceAsync(1)).ReturnsAsync((Session)null);

            var dto = new CreatePrescriptionDto { SessionId = 1 };

            await Assert.ThrowsAsync<Exception>(() => _service.CreatePrescriptionAsync(dto));
        }

        [Fact]
        public async Task CreatePrescriptionAsync_PatientAbsent_ThrowsException()
        {
            var session = new Session { Id = 1, Attendance = null, Status = SessionStatus.Confirmed };
            _sessionRepoMock.Setup(r => r.GetByIdWithAttendanceAsync(1)).ReturnsAsync(session);

            var dto = new CreatePrescriptionDto { SessionId = 1 };

            await Assert.ThrowsAsync<Exception>(() => _service.CreatePrescriptionAsync(dto));
        }

        [Fact]
        public async Task CreatePrescriptionAsync_SessionNotConfirmed_ThrowsException()
        {
            var session = new Session { Id = 1, Attendance = new Attendance { IsPresent = true }, Status = SessionStatus.Scheduled };
            _sessionRepoMock.Setup(r => r.GetByIdWithAttendanceAsync(1)).ReturnsAsync(session);

            var dto = new CreatePrescriptionDto { SessionId = 1 };

            await Assert.ThrowsAsync<Exception>(() => _service.CreatePrescriptionAsync(dto));
        }

        [Fact]
        public async Task CreatePrescriptionAsync_PrescriptionAlreadyExists_ThrowsException()
        {
            var session = new Session { Id = 1, Attendance = new Attendance { IsPresent = true }, Status = SessionStatus.Confirmed };
            _sessionRepoMock.Setup(r => r.GetByIdWithAttendanceAsync(1)).ReturnsAsync(session);
            _prescriptionRepoMock.Setup(r => r.GetBySessionIdAsync(1)).ReturnsAsync(new List<Prescription> { new Prescription() });

            var dto = new CreatePrescriptionDto { SessionId = 1 };

            await Assert.ThrowsAsync<Exception>(() => _service.CreatePrescriptionAsync(dto));
        }

        [Fact]
        public async Task CreatePrescriptionAsync_ValidInput_CreatesPrescription()
        {
            var session = new Session { Id = 1, Attendance = new Attendance { IsPresent = true }, Status = SessionStatus.Confirmed };
            _sessionRepoMock.Setup(r => r.GetByIdWithAttendanceAsync(1)).ReturnsAsync(session);
            _prescriptionRepoMock.Setup(r => r.GetBySessionIdAsync(1)).ReturnsAsync(new List<Prescription>());
            var prescription = new Prescription { Id = 1 };
            var dto = new CreatePrescriptionDto { SessionId = 1, Notes = "Notes", Diagnosis = "Diagnosis" };

            _mapperMock.Setup(m => m.Map<Prescription>(dto)).Returns(prescription);
            _mapperMock.Setup(m => m.Map<PrescriptionDto>(prescription)).Returns(new PrescriptionDto { Id = 1 });

            _prescriptionRepoMock.Setup(r => r.AddAsync(prescription)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _medicalRecordServiceMock.Setup(m => m.CreateMedicalRecordAsync(dto.Notes, dto.Diagnosis, prescription.Id)).ReturnsAsync(1);

            var result = await _service.CreatePrescriptionAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            _prescriptionRepoMock.Verify(r => r.AddAsync(prescription), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            _medicalRecordServiceMock.Verify(m => m.CreateMedicalRecordAsync(dto.Notes, dto.Diagnosis, prescription.Id), Times.Once);
        }
    }
}