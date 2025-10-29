using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagement.Test
{
    public class DoctorServiceTest
    {
        private readonly IDoctorService service;
        private readonly Mock<IDoctorRepository> repo;
        private readonly Mock<IUnitOfWork> unitOfWork;
        public DoctorServiceTest() {
            unitOfWork = new Mock<IUnitOfWork>();
            repo = new Mock<IDoctorRepository>();
            unitOfWork.Setup(u => u.DoctorRepository).Returns(repo.Object);
            service = new DoctorService(repo.Object,unitOfWork.Object);
        }
        [Fact]
        public async Task GetDoctorsBySpecializationAsync_ValidSpecializationId_ReturnsDoctors()
        {
            repo.Setup(r => r.GetAllBySpecializationAsync(1)).ReturnsAsync(new List<Doctor>
            {
                new Doctor { Id = "D1", SpecializationId = 1 },
                new Doctor { Id = "D2",  SpecializationId = 1 }
            });
            var result = await service.GetDoctorsBySpecializationAsync(1);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, d => Assert.Equal(d.SpecializationId, 1));

            repo.Verify(r => r.GetAllBySpecializationAsync(1), Times.Once);
        }
        [Fact]
        public async Task GetDoctorsBySpecializationAsync_NoDoctors_ReturnsEmptyList()
        {

            repo.Setup(r => r.GetAllBySpecializationAsync(2)).ReturnsAsync(new List<Doctor>());

            var result = await service.GetDoctorsBySpecializationAsync(2);

            Assert.NotNull(result);
            Assert.Empty(result);

            repo.Verify(r => r.GetAllBySpecializationAsync(2), Times.Once);
        }
        [Fact]
        public async Task GetDoctorsBySpecializationAsync_InvalidSpecializationId_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetDoctorsBySpecializationAsync(0));
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetDoctorsBySpecializationAsync(-1));
        }
    }
}
