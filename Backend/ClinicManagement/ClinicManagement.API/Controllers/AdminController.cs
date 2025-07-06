using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AdminDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuditLoggerService auditLogger;
        private string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public AdminController(IUnitOfWork _unitOfWork, IMapper _mapper, IAuditLoggerService _auditLogger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            auditLogger = _auditLogger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admins = await unitOfWork.AdminRepository.GetAllAsync();
            if (admins == null || !admins.Any())
            {
                return NotFound("No admins found.");
            }
            var adminDtos = mapper.Map<IEnumerable<AdminDto>>(admins);
            return Ok(adminDtos);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdminById(string id)
        {
            var admin = await unitOfWork.AdminRepository.GetByIdAsync(id);
            if (admin == null)
            {
                return NotFound($"Admin with ID {id} not found.");
            }
            var adminDto = mapper.Map<AdminDto>(admin);
            return Ok(adminDto);
        }
        [HttpGet("Email/{email}")]
        public async Task<IActionResult> GetAdminByEmail(string email)
        {
            var admin = await unitOfWork.AdminRepository.GetByEmailAsync(email);
            if (admin == null)
            {
                return NotFound($"Admin with Email {email} not found.");
            }
            var adminDto = mapper.Map<AdminDto>(admin);
            return Ok(adminDto);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdmin(string id, [FromBody] AdminDto adminDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (adminDto == null)
            {
                return BadRequest("Admin data is null.");
            }
            var admin = await unitOfWork.AdminRepository.GetByIdAsync(id);
            if (admin == null)
            {
                return NotFound($"Admin with ID {id} not found.");
            }
            mapper.Map(adminDto, admin);
            unitOfWork.AdminRepository.Update(admin);
            await unitOfWork.SaveChangesAsync();

            // Log the update of the admin
            await auditLogger.LogAsync("Update", "Admin", $"Updated admin with ID {admin.Id}", UserId);
            var resultDto = mapper.Map<AdminDto>(admin);
            return Ok(resultDto);
        }
        [HttpPut("Deactivate/{id}")]
        public async Task<IActionResult> DeactivateAdmin(string id)
        {
            var admin = await unitOfWork.AdminRepository.GetByIdAsync(id);
            if (admin == null)
            {
                return NotFound($"Admin with ID {id} not found.");
            }
            await unitOfWork.AdminRepository.DeactivateUserAsync(id);
            await unitOfWork.SaveChangesAsync();

            // Log the deactivation of the admin
            await auditLogger.LogAsync("Deactivate", "Admin", $"Deactivated admin with ID {id}", UserId);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            var admin = await unitOfWork.AdminRepository.GetByIdAsync(id);
            if (admin == null)
            {
                return NotFound($"Admin with ID {id} not found.");
            }
            unitOfWork.AdminRepository.Delete(admin);
            await unitOfWork.SaveChangesAsync();

            // Log the deletion of the admin
            await auditLogger.LogAsync("Delete", "Admin", $"Deleted admin with ID {admin.Id}", UserId);
            return NoContent();
        }

    }
}
