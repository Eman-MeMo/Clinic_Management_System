using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.Users.Patient.GetPatientByNationalId;
using ClinicManagement.Domain.DTOs.PatientDTOs;
using ClinicManagement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Queries.Users
{
    public class GetPatientByNationalIdHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPatientRepository> _patientRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetPatientByNationalIdHandler _handler;

        public GetPatientByNationalIdHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _patientRepoMock = new Mock<IPatientRepository>();
            _mapperMock = new Mock<IMapper>();

            _unitOfWorkMock.Setup(u => u.PatientRepository).Returns(_patientRepoMock.Object);

            _handler = new GetPatientByNationalIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsMappedPatient()
        {
            var query = new GetPatientByNationalIdQuery { NationalId = "1234567890" };
            var patient = new Patient { Id = "p1", FirstName = "John", NationID = "1234567890" };
            var patientDto = new PatientDto { Id = "p1", FirstName = "John", NationID = "1234567890" };

            _patientRepoMock.Setup(r => r.GetByNationalIdAsync(query.NationalId))
                            .ReturnsAsync(patient);

            _mapperMock.Setup(m => m.Map<PatientDto>(patient))
                       .Returns(patientDto);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(patientDto, result);
            _patientRepoMock.Verify(r => r.GetByNationalIdAsync(query.NationalId), Times.Once);
            _mapperMock.Verify(m => m.Map<PatientDto>(patient), Times.Once);
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
        }
    }
}