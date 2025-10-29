using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Services
{
    public class PatientServiceTest
    {
        private readonly Mock<IPatientRepository> repo;
        private readonly Mock<IUnitOfWork> unitOfWork;
        private readonly IPatientService patientService;
        public PatientServiceTest()
        {
            repo = new Mock<IPatientRepository>();
            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(u=>u.PatientRepository).Returns(repo.Object);
            patientService = new PatientService(repo.Object, unitOfWork.Object);
        }
        [Fact]
        public async Task GetByNationalIdAsync_ValidNationalId_ReturnPatient()
        {
            repo.Setup(p => p.GetByNationalIdAsync("11111111111")).ReturnsAsync(
                new Patient()
                {
                    FirstName = "Ali",
                    LastName = "Said",
                    gender = Gender.Male,
                    DateOfBirth = new DateOnly(2001, 11, 11)
                });

            var result = await patientService.GetByNationalIdAsync("11111111111");

            Assert.NotNull(result);
            repo.Verify(r => r.GetByNationalIdAsync("11111111111"),Times.Once);
        }
        [Fact]
        public async Task GetByNationalIdAsync_NoPatient_ReturnEmptyPatient()
        {
            repo.Setup(r=>r.GetByNationalIdAsync("123456789")).ReturnsAsync((Patient)null);

            var result = await patientService.GetByNationalIdAsync("123456789");

            Assert.Null(result);
        }
        [Fact]
        public async Task GetByNationalIdAsync_InValidNationalId_ReturnPatient()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => patientService.GetByNationalIdAsync(null));
        }
    }
}
