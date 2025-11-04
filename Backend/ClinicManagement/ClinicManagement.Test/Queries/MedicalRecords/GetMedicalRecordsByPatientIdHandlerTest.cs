using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.MedicalRecords.GetMedicalRecordsByPatientId;
using ClinicManagement.Domain.DTOs.MedicalRecordDTOs;
using ClinicManagement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Queries.MedicalRecords
{
    public class GetMedicalRecordsByPatientIdHandlerTests
    {
        private readonly Mock<IUnitOfWork> mockUnitOfWork;
        private readonly Mock<IMapper> mockMapper;
        private readonly GetMedicalRecordsByPatientIdHandler handler;

        public GetMedicalRecordsByPatientIdHandlerTests()
        {
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockMapper = new Mock<IMapper>();
            handler = new GetMedicalRecordsByPatientIdHandler(mockUnitOfWork.Object, mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ValidPatientId_ReturnsMappedMedicalRecords()
        {
            string patientId = "P1";
            var query = new GetMedicalRecordsByPatientIdQuery() { PatientId = patientId };

            var records = new List<MedicalRecord>
            {
                new MedicalRecord { Id = 1, PatientId = patientId, Diagnosis = "Cold" },
                new MedicalRecord { Id = 2, PatientId = patientId, Diagnosis = "Flu" }
            };

            var mappedDtos = new List<MedicalRecordDto>
            {
                new MedicalRecordDto { Id = 1, Diagnosis = "Cold" },
                new MedicalRecordDto { Id = 2, Diagnosis = "Flu" }
            };

            mockUnitOfWork
                .Setup(u => u.MedicalRecordRepository.GetByPatientIdAsync(patientId))
                .ReturnsAsync(records);

            mockMapper
                .Setup(m => m.Map<IEnumerable<MedicalRecordDto>>(records))
                .Returns(mappedDtos);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, ((List<MedicalRecordDto>)result).Count);
            Assert.Collection(result,
                item => Assert.Equal("Cold", item.Diagnosis),
                item => Assert.Equal("Flu", item.Diagnosis));
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            GetMedicalRecordsByPatientIdQuery? query = null;
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_EmptyPatientId_ReturnsEmptyList()
        {
            var query = new GetMedicalRecordsByPatientIdQuery() { PatientId = string.Empty };

            mockUnitOfWork
                .Setup(u => u.MedicalRecordRepository.GetByPatientIdAsync(string.Empty))
                .ReturnsAsync(new List<MedicalRecord>());

            mockMapper
                .Setup(m => m.Map<IEnumerable<MedicalRecordDto>>(It.IsAny<IEnumerable<MedicalRecord>>()))
                .Returns(new List<MedicalRecordDto>());

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}