using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClinicManagement.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        private readonly string[] allowedRoles = new[] { "Admin", "Doctor", "Patient" };

        public AccountService(UserManager<AppUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<object> LoginAsync(string Email, string Password)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null || !user.IsActive)
                return new { success = false, message = "Invalid username or password." };

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, Password);
            if (!isPasswordCorrect)
                return new { success = false, message = "Invalid username or password." };

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (string.IsNullOrEmpty(role) || !allowedRoles.Contains(role))
                return new { success = false, message = "User does not have a valid role." };

            var token = GenerateJWTAuthentication(user, role);

            return new { success = true, message = "Login successful", token };
        }

        public async Task<object> RegisterPatientAsync(string FirstName, string Lastname, string PhoneNumber, string Email, string Password, string ConfirmPassword,string NationId,Gender gender,DateOnly dateOfBirth)
        {
            var patient = new Patient
            {
                FirstName = FirstName,
                LastName = Lastname,
                PhoneNumber = PhoneNumber,
                Email = Email,
                UserName = Email,
                DateOfBirth = dateOfBirth,
                NationID = NationId,
                IsActive = true,
            };
            var result = await _userManager.CreateAsync(patient, Password);
            if (!result.Succeeded)
                return new { success = false, errors = result.Errors.Select(e => e.Description) };

            await _userManager.AddToRoleAsync(patient, "Patient");
            var token = GenerateJWTAuthentication(patient, "Patient");

            return new { success = true, message = "Patient registered successfully", token };
        }

        public async Task<object> RegisterDoctorAsync(string FirstName, string Lastname, string PhoneNumber, string Email, string Password, string ConfirmPassword,int SpecializationId)
        {
            var doctor = new Doctor
            {
                UserName = Email,
                Email = Email,
                FirstName = FirstName,
                LastName = Lastname,
                PhoneNumber = PhoneNumber,
                IsActive = true,
                SpecializationId = SpecializationId
            };

            var result = await _userManager.CreateAsync(doctor, Password);
            if (!result.Succeeded)
                return new { success = false, errors = result.Errors.Select(e => e.Description) };

            await _userManager.AddToRoleAsync(doctor, "Doctor");
            var token = GenerateJWTAuthentication(doctor, "Doctor");

            return new { success = true, message = "Doctor registered successfully", token };
        }

        public async Task<object> RegisterAdminAsync(string FirstName, string Lastname, string PhoneNumber, string Email, string Password, string ConfirmPassword)
        {
            var admin = new Admin
            {
                UserName = Email,
                Email = Email,
                FirstName = FirstName,
                LastName = Lastname,
                PhoneNumber = PhoneNumber,
                IsActive = true
            };
            var result = await _userManager.CreateAsync(admin, Password);
            if (!result.Succeeded)
                return new { success = false, errors = result.Errors.Select(e => e.Description) };

            await _userManager.AddToRoleAsync(admin, "Admin");
            var token = GenerateJWTAuthentication(admin, "Admin");

            return new { success = true, message = "Admin registered successfully", token };
        }

        private string GenerateJWTAuthentication(AppUser user, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: DateTime.UtcNow.AddHours(double.Parse(_config["Jwt:DurationInHours"])),
                claims: claims,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
