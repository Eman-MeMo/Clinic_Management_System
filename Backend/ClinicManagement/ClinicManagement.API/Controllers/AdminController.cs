using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AdminDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<AdminController> logger;

        public AdminController(IUnitOfWork _unitOfWork, IMapper _mapper, ILogger<AdminController> _logger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            logger = _logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAdmins()
        {
            logger.LogInformation("Fetching all admins");
            var admins = await unitOfWork.AdminRepository.GetAllAsync();
            if (admins == null || !admins.Any())
            {
                logger.LogWarning("No admins found.");
                return NotFound("No admins found.");
            }
            var adminDtos = mapper.Map<IEnumerable<AdminDto>>(admins);
            logger.LogInformation("{Count} admins retrieved successfully", adminDtos.Count());
            return Ok(adminDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdminById(string id)
        {
            logger.LogInformation("Fetching admin with ID: {Id}", id);
            var admin = await unitOfWork.AdminRepository.GetByIdAsync(id);
            if (admin == null)
            {
                logger.LogWarning("Admin with ID {Id} not found.", id);
                return NotFound($"Admin with ID {id} not found.");
            }
            var adminDto = mapper.Map<AdminDto>(admin);
            logger.LogInformation("Admin with ID {Id} retrieved successfully", id);
            return Ok(adminDto);
        }

        [HttpGet("Email/{email}")]
        public async Task<IActionResult> GetAdminByEmail(string email)
        {
            logger.LogInformation("Fetching admin with Email: {Email}", email);
            var admin = await unitOfWork.AdminRepository.GetByEmailAsync(email);
            if (admin == null)
            {
                logger.LogWarning("Admin with Email {Email} not found.", email);
                return NotFound($"Admin with Email {email} not found.");
            }
            var adminDto = mapper.Map<AdminDto>(admin);
            logger.LogInformation("Admin with Email {Email} retrieved successfully", email);
            return Ok(adminDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdmin(string id, [FromBody] AdminDto adminDto)
        {
            logger.LogInformation("Updating admin with ID: {Id}", id);
            if (adminDto == null)
            {
                logger.LogWarning("Admin data is null.");
                return BadRequest("Admin data is null.");
            }
            var admin = await unitOfWork.AdminRepository.GetByIdAsync(id);
            if (admin == null)
            {
                logger.LogWarning("Admin with ID {Id} not found.", id);
                return NotFound($"Admin with ID {id} not found.");
            }
            mapper.Map(adminDto, admin);
            unitOfWork.AdminRepository.Update(admin);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Admin with ID {Id} updated successfully", id);

            var resultDto = mapper.Map<AdminDto>(admin);
            return Ok(resultDto);
        }

        [HttpPut("Deactivate/{id}")]
        public async Task<IActionResult> DeactivateAdmin(string id)
        {
            logger.LogInformation("Deactivating admin with ID: {Id}", id);
            var admin = await unitOfWork.AdminRepository.GetByIdAsync(id);
            if (admin == null)
            {
                logger.LogWarning("Admin with ID {Id} not found.", id);
                return NotFound($"Admin with ID {id} not found.");
            }
            await unitOfWork.AdminRepository.DeactivateUserAsync(id);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Admin with ID {Id} deactivated successfully", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            logger.LogInformation("Deleting admin with ID: {Id}", id);
            var admin = await unitOfWork.AdminRepository.GetByIdAsync(id);
            if (admin == null)
            {
                logger.LogWarning("Admin with ID {Id} not found.", id);
                return NotFound($"Admin with ID {id} not found.");
            }
            unitOfWork.AdminRepository.Delete(admin);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Admin with ID {Id} deleted successfully", id);
            return NoContent();
        }
    }
}
