using AutoMapper;
using ClinicManagement.Domain.DTOs;
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
        private readonly SignInManager<AppUser> signInManager;
        private readonly IMapper mapper;
        private readonly IConfiguration config;
        private readonly ClinicDbContext db;

        private readonly string[] allowedRoles = new[] { "Admin", "Doctor", "Patient" };

        public AccountController(UserManager<AppUser> _userManager,
                                 SignInManager<AppUser> _signInManager,
                                 IMapper _mapper,
                                 IConfiguration _config,
                                 ClinicDbContext _db)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            mapper = _mapper;
            config = _config;
            db = _db;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByEmailAsync(model.Username)
                    ?? await userManager.FindByNameAsync(model.Username);

            if (user == null || !user.IsActive)
                return Unauthorized("Invalid username or password.");

            var isPasswordCorrect = await userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordCorrect)
                return Unauthorized("Invalid username or password.");

            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (string.IsNullOrEmpty(role) || !allowedRoles.Contains(role))
                return Unauthorized("User does not have a valid role.");

            var token = GenerateJWTAuthentication(user, role);
            LogAudit("Login", user.Id, $"User '{user.UserName}' with role '{role}' logged in.");

            return Ok(new
            {
                message = "Login successful",
                token = token
            });
        }

        [HttpPost("Patient_Register")]
        public async Task<IActionResult> PatientRegister([FromBody] PatientRegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var patient = mapper.Map<Patient>(model);
            var result = await userManager.CreateAsync(patient, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var roleResult = await userManager.AddToRoleAsync(patient, "Patient");
            if (!roleResult.Succeeded)
                return BadRequest("Failed to assign Patient role.");

            var token = GenerateJWTAuthentication(patient, "Patient");
            LogAudit("Register", patient.Id, $"New user '{patient.UserName}' registered as Patient.");

            return Ok(new { message = "Registered successfully", token });
        }

        [HttpPost("Doctor_Register")]
        public async Task<IActionResult> DoctorRegister([FromBody] DoctorRegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var doctor = mapper.Map<Doctor>(model);
            var result = await userManager.CreateAsync(doctor, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var roleResult = await userManager.AddToRoleAsync(doctor, "Doctor");
            if (!roleResult.Succeeded)
                return BadRequest("Failed to assign Doctor role.");

            var token = GenerateJWTAuthentication(doctor, "Doctor");
            LogAudit("Register", doctor.Id, $"New user '{doctor.UserName}' registered as Doctor.");

            return Ok(new { message = "Registered successfully", token });
        }

        [HttpPost("Admin_Register")]
        public async Task<IActionResult> AdminRegister([FromBody] AdminRegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var admin = mapper.Map<Admin>(model);
            var result = await userManager.CreateAsync(admin, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var roleResult = await userManager.AddToRoleAsync(admin, "Admin");
            if (!roleResult.Succeeded)
                return BadRequest("Failed to assign Admin role.");

            var token = GenerateJWTAuthentication(admin, "Admin");
            LogAudit("Register", admin.Id, $"New user '{admin.UserName}' registered as Admin.");

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

        private void LogAudit(string action, string userId, string details)
        {
            db.AuditLogs.Add(new AuditLog
            {
                Action = action,
                TableName = "AppUser",
                Timestamp = DateTime.UtcNow,
                Details = details,
                AppUserId = userId
            });

            db.SaveChanges();
        }
    }
}
