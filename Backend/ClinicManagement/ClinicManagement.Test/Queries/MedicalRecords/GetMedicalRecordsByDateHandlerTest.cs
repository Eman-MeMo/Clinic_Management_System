using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.MedicalRecords.GetMedicalRecordsByDate;
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
    public class GetMedicalRecordsByDateHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetMedicalRecordsByDateHandler _handler;

        public GetMedicalRecordsByDateHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetMedicalRecordsByDateHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedMedicalRecords_WhenValidRequest()
        {
            var query = new GetMedicalRecordsByDateQuery
            {
                date = new DateTime(2025, 10, 27)
            };

            var medicalRecords = new List<MedicalRecord>
        {
            new MedicalRecord { Id = 1, Diagnosis = "Flu" },
            new MedicalRecord { Id = 2, Diagnosis = "Cold" }
        };

            var mappedDtos = new List<MedicalRecordDto>
        {
            new MedicalRecordDto { Id = 1, Diagnosis = "Flu" },
            new MedicalRecordDto { Id = 2, Diagnosis = "Cold" }
        };

            _unitOfWorkMock.Setup(u => u.MedicalRecordRepository.GetByDateAsync(query.date))
                           .ReturnsAsync(medicalRecords);

            _mapperMock.Setup(m => m.Map<IEnumerable<MedicalRecordDto>>(medicalRecords))
                       .Returns(mappedDtos);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, ((List<MedicalRecordDto>)result).Count);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoRecordsFound()
        {
            var query = new GetMedicalRecordsByDateQuery
            {
                date = new DateTime(2025, 10, 27)
            };

            _unitOfWorkMock.Setup(u => u.MedicalRecordRepository.GetByDateAsync(query.date))
                           .ReturnsAsync(new List<MedicalRecord>());

            _mapperMock.Setup(m => m.Map<IEnumerable<MedicalRecordDto>>(It.IsAny<IEnumerable<MedicalRecord>>()))
                       .Returns(new List<MedicalRecordDto>());

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
