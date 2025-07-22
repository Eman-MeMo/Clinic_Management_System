using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AccountDTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IMapper mapper;
        private readonly IConfiguration config;
        private readonly ILogger<AccountController> logger;

        private readonly string[] allowedRoles = new[] { "Admin", "Doctor", "Patient" };

        public AccountController(UserManager<AppUser> _userManager,
                                 SignInManager<AppUser> _signInManager,
                                 IMapper _mapper,
                                 IConfiguration _config,
                                 ILogger<AccountController> _logger)
        {
            userManager = _userManager;
            mapper = _mapper;
            config = _config;
            logger = _logger;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            logger.LogInformation("Login attempt for email: {Email}", model.Email);

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.IsActive)
            {
                logger.LogWarning("Login failed for email: {Email}. Reason: User not found or inactive.", model.Email);
                return Unauthorized("Invalid username or password.");
            }

            var isPasswordCorrect = await userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordCorrect)
            {
                logger.LogWarning("Login failed for email: {Email}. Reason: Incorrect password.", model.Email);
                return Unauthorized("Invalid username or password.");
            }

            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (string.IsNullOrEmpty(role) || !allowedRoles.Contains(role))
            {
                logger.LogWarning("Login failed for email: {Email}. Reason: Invalid role.", model.Email);
                return Unauthorized("User does not have a valid role.");
            }

            var token = GenerateJWTAuthentication(user, role);

            logger.LogInformation("User {Email} logged in successfully with role {Role}.", model.Email, role);

            return Ok(new
            {
                message = "Login successful",
                token = token
            });
        }

        [HttpPost("Patient_Register")]
        public async Task<IActionResult> PatientRegister([FromBody] PatientRegisterDto model)
        {
            logger.LogInformation("Patient registration attempt for email: {Email}", model.Email);

            var patient = mapper.Map<Patient>(model);
            var result = await userManager.CreateAsync(patient, model.Password);
            if (!result.Succeeded)
            {
                logger.LogError("Patient registration failed for email: {Email}. Errors: {Errors}", model.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return BadRequest(result.Errors);
            }

            var roleResult = await userManager.AddToRoleAsync(patient, "Patient");
            if (!roleResult.Succeeded)
            {
                logger.LogError("Failed to assign Patient role to user: {Email}", model.Email);
                return BadRequest("Failed to assign Patient role.");
            }

            var token = GenerateJWTAuthentication(patient, "Patient");

            logger.LogInformation("Patient registered successfully: {Email}", model.Email);

            return Ok(new { message = "Registered successfully", token });
        }

        [HttpPost("Doctor_Register")]
        public async Task<IActionResult> DoctorRegister([FromBody] DoctorRegisterDto model)
        {
            logger.LogInformation("Doctor registration attempt for email: {Email}", model.Email);

            var doctor = mapper.Map<Doctor>(model);
            var result = await userManager.CreateAsync(doctor, model.Password);
            if (!result.Succeeded)
            {
                logger.LogError("Doctor registration failed for email: {Email}. Errors: {Errors}", model.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return BadRequest(result.Errors);
            }

            var roleResult = await userManager.AddToRoleAsync(doctor, "Doctor");
            if (!roleResult.Succeeded)
            {
                logger.LogError("Failed to assign Doctor role to user: {Email}", model.Email);
                return BadRequest("Failed to assign Doctor role.");
            }

            var token = GenerateJWTAuthentication(doctor, "Doctor");

            logger.LogInformation("Doctor registered successfully: {Email}", model.Email);

            return Ok(new { message = "Registered successfully", token });
        }

        [HttpPost("Admin_Register")]
        public async Task<IActionResult> AdminRegister([FromBody] AdminRegisterDto model)
        {
            logger.LogInformation("Admin registration attempt for email: {Email}", model.Email);

            var admin = mapper.Map<Admin>(model);
            var result = await userManager.CreateAsync(admin, model.Password);
            if (!result.Succeeded)
            {
                logger.LogError("Admin registration failed for email: {Email}. Errors: {Errors}", model.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return BadRequest(result.Errors);
            }

            var roleResult = await userManager.AddToRoleAsync(admin, "Admin");
            if (!roleResult.Succeeded)
            {
                logger.LogError("Failed to assign Admin role to user: {Email}", model.Email);
                return BadRequest("Failed to assign Admin role.");
            }

            var token = GenerateJWTAuthentication(admin, "Admin");

            logger.LogInformation("Admin registered successfully: {Email}", model.Email);

            return Ok(new { message = "Registered successfully", token });
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                expires: DateTime.UtcNow.AddHours(double.Parse(config["Jwt:DurationInHours"])),
                claims: claims,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
