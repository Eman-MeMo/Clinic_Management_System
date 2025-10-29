using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Data;
using ClinicManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test
{
    public class MedicalRecordServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPrescriptionRepository> _prescriptionRepoMock;
        private readonly Mock<IMedicalRecordRepository> _medicalRecordRepoMock;
        private readonly IMedicalRecordService _service;
        public MedicalRecordServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _prescriptionRepoMock = new Mock<IPrescriptionRepository>();
            _medicalRecordRepoMock = new Mock<IMedicalRecordRepository>();
            _unitOfWorkMock.Setup(u => u.PrescriptionRepository).Returns(_prescriptionRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.MedicalRecordRepository).Returns(_medicalRecordRepoMock.Object);
            _service = new MedicalRecordService(_unitOfWorkMock.Object);
        }
        [Fact]
        public async Task CreateMedicalRecordAsync_ValidPrescriptionId_CreatesMedicalRecord()
        {
            var options = new DbContextOptionsBuilder<ClinicDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var loggerMock = new Mock<ILogger<ClinicDbContext>>();
            using (var context = new ClinicDbContext(options, loggerMock.Object))
            {
                var prescription = new Prescription
                {
                    Id = 1,
                    MedicationName = "Med1",
                    Dosage = "10mg",
                    Session = new Session
                    {
                        Id = 1,
                        PatientId = "P1",
                        DoctorId = "D1",
                        Appointment = new Appointment
                        {
                            Date = DateTime.Today,
                            PatientId = "P1",
                            DoctorId = "D1",
                            Notes = "Regular checkup"
                        }
                    }
                };
                context.Prescriptions.Add(prescription);
                context.SaveChanges();

                var prescriptionId = prescription.Id;
                var notes = "Patient shows symptoms of flu.";
                var diagnosis = "Influenza";

                var unitOfWork = new UnitOfWork(context);
                var service = new MedicalRecordService(unitOfWork);

                var result = await service.CreateMedicalRecordAsync(notes, diagnosis, prescriptionId);

                Assert.True(result >= 0);
            }
        }
        [Fact]
        public async Task CreateMedicalRecordAsync_InvalidPrescriptionId_ThrowsException()
        {
            var invalidPrescriptionId = 999;
            var notes = "Some notes";
            var diagnosis = "Some diagnosis";

            _prescriptionRepoMock.Setup(p => p.GetAllAsQueryable())
                .Returns(new List<Prescription>().AsQueryable());

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.CreateMedicalRecordAsync(notes, diagnosis, invalidPrescriptionId));
        }
        [Fact]
        public async Task CreateMedicalRecordAsync_NullSession_ThrowsException()
        {
            var prescription = new Prescription
            {
                Id = 1,
                MedicationName = "Med1",
                Dosage = "10mg",
                Session = null,
            };

            _prescriptionRepoMock.Setup(p => p.GetAllAsQueryable())
                .Returns(new List<Prescription> { prescription }.AsQueryable());

            var notes = "Some notes";
            var diagnosis = "Some diagnosis";
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.CreateMedicalRecordAsync(notes, diagnosis, prescription.Id));
        }
        [Fact]
        public async Task CreateMedicalRecordAsync_NullAppointment_ThrowsException()
        {
            var prescription = new Prescription
            {
                Id = 1,
                MedicationName = "Med1",
                Dosage = "10mg",
                Session = new Session
                {
                    Id = 1,
                    PatientId = "P1",
                    DoctorId = "D1",
                    Appointment = null
                }
            };
            _prescriptionRepoMock.Setup(p => p.GetAllAsQueryable())
                .Returns(new List<Prescription> { prescription }.AsQueryable());

            var notes = "Some notes";
            var diagnosis = "Some diagnosis";
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.CreateMedicalRecordAsync(notes, diagnosis, prescription.Id));
        }
    }
}
