using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.MedicalRecords.GetLatestMedicalRecordByPatientId;
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
    public class GetLatestMedicalRecordByPatientIdHandlerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetLatestMedicalRecordByPatientIdHandler _handler;

        public GetLatestMedicalRecordByPatientIdHandlerTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetLatestMedicalRecordByPatientIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedMedicalRecord_WhenValidRequest()
        {
            var patientId = "p5";
            var query = new GetLatestMedicalRecordByPatientIdQuery() { PatientId = patientId };
            var medicalRecord = new MedicalRecord { Id = 10, PatientId = patientId };
            var mappedDto = new MedicalRecordDto { Id = 10 };

            _unitOfWorkMock.Setup(u => u.MedicalRecordRepository.GetLatestRecordAsync(patientId))
                           .ReturnsAsync(medicalRecord);

            _mapperMock.Setup(m => m.Map<MedicalRecordDto>(medicalRecord))
                       .Returns(mappedDto);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(mappedDto.Id, result.Id);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenNoMedicalRecordFound()
        {
            var query = new GetLatestMedicalRecordByPatientIdQuery() { PatientId = "p10" };

            _unitOfWorkMock.Setup(u => u.MedicalRecordRepository.GetLatestRecordAsync("p10"))
                           .ReturnsAsync((MedicalRecord)null);

            _mapperMock.Setup(m => m.Map<MedicalRecordDto>(null))
                       .Returns((MedicalRecordDto)null);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Null(result);
        }
    }
}