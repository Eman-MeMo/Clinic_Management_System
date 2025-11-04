using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.Users.Doctor.GetDoctorBySpecilation;
using ClinicManagement.Domain.DTOs.DoctorDTOs;
using ClinicManagement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Queries.Users
{
    public class GetDoctorBySpecilationHandlerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IDoctorRepository> _doctorRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetDoctorBySpecilationHandler _handler;

        public GetDoctorBySpecilationHandlerTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _doctorRepoMock = new Mock<IDoctorRepository>();
            _mapperMock = new Mock<IMapper>();

            _unitOfWorkMock.Setup(u => u.DoctorRepository).Returns(_doctorRepoMock.Object);

            _handler = new GetDoctorBySpecilationHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsMappedDoctors()
        {
            var query = new GetDoctorBySpecilationQuery { SpecializationId = 1 };

            var doctors = new List<Doctor>
            {
                new Doctor { Id = "d1", FirstName = "John" },
                new Doctor { Id = "d2", FirstName = "Eman" }
            };

            var doctorDtos = new List<DoctorDto>
            {
                new DoctorDto { Id = "d1", FirstName = "John" },
                new DoctorDto { Id = "d2", FirstName = "Eman" }
            };

            _doctorRepoMock
                .Setup(r => r.GetAllBySpecializationAsync(query.SpecializationId))
                .ReturnsAsync(doctors);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<DoctorDto>>(doctors))
                .Returns(doctorDtos);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(doctorDtos, result);
            _doctorRepoMock.Verify(r => r.GetAllBySpecializationAsync(query.SpecializationId), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<DoctorDto>>(doctors), Times.Once);
        }

        [Fact]
        public async Task Handle_NullRequest_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
        }
    }
}