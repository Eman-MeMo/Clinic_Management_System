using Xunit;
using Moq;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClinicManagement.Domain.Enums;
using System.Reflection;

namespace ClinicManagement.Test.Services
{
    public class AccountServiceTest
    {
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<IConfiguration> _configMock;

        public AccountServiceTest()
        {
            var store = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);

            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(c => c["Jwt:Key"]).Returns("supersecretkey1234567890super1234");
            _configMock.Setup(c => c["Jwt:Issuer"]).Returns("testIssuer");
            _configMock.Setup(c => c["Jwt:Audience"]).Returns("testAudience");
            _configMock.Setup(c => c["Jwt:DurationInHours"]).Returns("1");
        }

        #region LoginAsync Tests
        [Fact]
        public async Task LoginAsync_ValidUser_ReturnsSuccess()
        {
            var user = new AppUser { Id = "1", Email = "test@test.com", FirstName = "John", LastName = "Doe", IsActive = true };
            _userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com")).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "password")).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Patient" });

            var service = new AccountService(_userManagerMock.Object, _configMock.Object);

            var result = await service.LoginAsync("test@test.com", "password");

            Assert.NotNull(result);
            var success = (bool)result.GetType().GetProperty("success").GetValue(result);
            Assert.True(success);
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ReturnsFailure()
        {
            var user = new AppUser { Id = "1", Email = "test@test.com", IsActive = true };
            _userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com")).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "wrongpassword")).ReturnsAsync(false);

            var service = new AccountService(_userManagerMock.Object, _configMock.Object);

            var result = await service.LoginAsync("test@test.com", "wrongpassword");

            var success = (bool)result.GetType().GetProperty("success").GetValue(result);
            Assert.False(success);
        }

        [Fact]
        public async Task LoginAsync_UserWithInvalidRole_ReturnsFailure()
        {
            var user = new AppUser { Id = "1", Email = "test@test.com", IsActive = true };
            _userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com")).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "password")).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "InvalidRole" });

            var service = new AccountService(_userManagerMock.Object, _configMock.Object);

            var result = await service.LoginAsync("test@test.com", "password");

            var success = (bool)result.GetType().GetProperty("success").GetValue(result);
            Assert.False(success);
        }
        #endregion

        #region RegisterPatientAsync Tests
        [Fact]
        public async Task RegisterPatientAsync_ValidData_ReturnsSuccess()
        {
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<Patient>(), "Password123")).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<Patient>(), "Patient")).ReturnsAsync(IdentityResult.Success);

            var service = new AccountService(_userManagerMock.Object, _configMock.Object);

            var result = await service.RegisterPatientAsync("John", "Doe", "0123456789", "patient@test.com", "Password123", "Password123", "123456", Gender.Male, new System.DateOnly(1990, 1, 1));

            var success = (bool)result.GetType().GetProperty("success").GetValue(result);
            Assert.True(success);
        }

        [Fact]
        public async Task RegisterPatientAsync_CreateFails_ReturnsFailure()
        {
            var errors = new IdentityError[] { new IdentityError { Description = "Error creating user" } };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<Patient>(), "Password123")).ReturnsAsync(IdentityResult.Failed(errors));

            var service = new AccountService(_userManagerMock.Object, _configMock.Object);

            var result = await service.RegisterPatientAsync("John", "Doe", "0123456789", "patient@test.com", "Password123", "Password123", "123456", Gender.Male, new System.DateOnly(1990, 1, 1));

            var success = (bool)result.GetType().GetProperty("success").GetValue(result);
            Assert.False(success);
        }
        #endregion

        #region RegisterDoctorAsync Tests
        [Fact]
        public async Task RegisterDoctorAsync_ValidData_ReturnsSuccess()
        {
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<Doctor>(), "Password123")).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<Doctor>(), "Doctor")).ReturnsAsync(IdentityResult.Success);

            var service = new AccountService(_userManagerMock.Object, _configMock.Object);

            var result = await service.RegisterDoctorAsync("Alice", "Smith", "0123456789", "doctor@test.com", "Password123", "Password123", 1);

            var success = (bool)result.GetType().GetProperty("success").GetValue(result);
            Assert.True(success);
        }

        [Fact]
        public async Task RegisterDoctorAsync_CreateFails_ReturnsFailure()
        {
            var errors = new IdentityError[] { new IdentityError { Description = "Error creating doctor" } };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<Doctor>(), "Password123")).ReturnsAsync(IdentityResult.Failed(errors));

            var service = new AccountService(_userManagerMock.Object, _configMock.Object);

            var result = await service.RegisterDoctorAsync("Alice", "Smith", "0123456789", "doctor@test.com", "Password123", "Password123", 1);

            var success = (bool)result.GetType().GetProperty("success").GetValue(result);
            Assert.False(success);
        }
        #endregion

        #region RegisterAdminAsync Tests
        [Fact]
        public async Task RegisterAdminAsync_ValidData_ReturnsSuccess()
        {
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<Admin>(), "Password123")).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<Admin>(), "Admin")).ReturnsAsync(IdentityResult.Success);

            var service = new AccountService(_userManagerMock.Object, _configMock.Object);

            var result = await service.RegisterAdminAsync("Bob", "Johnson", "0123456789", "admin@test.com", "Password123", "Password123");

            var success = (bool)result.GetType().GetProperty("success").GetValue(result);
            Assert.True(success);
        }

        [Fact]
        public async Task RegisterAdminAsync_CreateFails_ReturnsFailure()
        {
            var errors = new IdentityError[] { new IdentityError { Description = "Error creating admin" } };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<Admin>(), "Password123")).ReturnsAsync(IdentityResult.Failed(errors));

            var service = new AccountService(_userManagerMock.Object, _configMock.Object);

            var result = await service.RegisterAdminAsync("Bob", "Johnson", "0123456789", "admin@test.com", "Password123", "Password123");

            var success = (bool)result.GetType().GetProperty("success").GetValue(result);
            Assert.False(success);
        }
        #endregion
    }

}