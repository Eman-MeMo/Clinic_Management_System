using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.DoctorDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<DoctorController> logger;

        public DoctorController(IUnitOfWork _unitOfWork, IMapper _mapper, ILogger<DoctorController> _logger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            logger = _logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            logger.LogInformation("Getting all doctors.");
            var doctors = await unitOfWork.DoctorRepository.GetAllAsync();
            if (doctors == null || !doctors.Any())
            {
                logger.LogWarning("No doctors found.");
                return NotFound("No doctors found.");
            }
            logger.LogInformation("Retrieved {Count} doctors.", doctors.Count());
            var doctorDtos = mapper.Map<IEnumerable<DoctorDto>>(doctors);
            return Ok(doctorDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(string id)
        {
            logger.LogInformation("Getting doctor by ID: {Id}", id);
            var doctor = await unitOfWork.DoctorRepository.GetByIdAsync(id);
            if (doctor == null)
            {
                logger.LogWarning("Doctor with ID {Id} not found.", id);
                return NotFound($"Doctor with ID {id} not found.");
            }
            logger.LogInformation("Doctor with ID {Id} retrieved successfully.", id);
            var doctorDto = mapper.Map<DoctorDto>(doctor);
            return Ok(doctorDto);
        }

        [HttpGet("Email/{email}")]
        public async Task<IActionResult> GetDoctorByEmail(string email)
        {
            logger.LogInformation("Getting doctor by email: {Email}", email);
            var doctor = await unitOfWork.DoctorRepository.GetByEmailAsync(email);
            if (doctor == null)
            {
                logger.LogWarning("Doctor with Email {Email} not found.", email);
                return NotFound($"Doctor with Email {email} not found.");
            }
            logger.LogInformation("Doctor with Email {Email} retrieved successfully.", email);
            var doctorDto = mapper.Map<DoctorDto>(doctor);
            return Ok(doctorDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(string id, [FromBody] DoctorDto doctorDto)
        {
            logger.LogInformation("Updating doctor with ID: {Id}", id);
            if (doctorDto == null || doctorDto.Id != id)
            {
                logger.LogWarning("Invalid data while updating doctor with ID: {Id}", id);
                return BadRequest("Invalid data!");
            }
            var doctor = await unitOfWork.DoctorRepository.GetByIdAsync(id);
            if (doctor == null)
            {
                logger.LogWarning("Doctor with ID {Id} not found for update.", id);
                return NotFound($"Doctor with ID {id} not found.");
            }

            mapper.Map(doctorDto, doctor);
            unitOfWork.DoctorRepository.Update(doctor);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Doctor with ID {Id} updated successfully.", id);
            var resultDto = mapper.Map<DoctorDto>(doctor);
            return Ok(resultDto);
        }

        [HttpPut("Deactivate/{id}")]
        public async Task<IActionResult> DeactivateDoctor(string id)
        {
            logger.LogInformation("Deactivating doctor with ID: {Id}", id);
            var doctor = await unitOfWork.DoctorRepository.GetByIdAsync(id);
            if (doctor == null)
            {
                logger.LogWarning("Doctor with ID {Id} not found for deactivation.", id);
                return NotFound($"Doctor with ID {id} not found.");
            }

            await unitOfWork.DoctorRepository.DeactivateUserAsync(id);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Doctor with ID {Id} deactivated successfully.", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            logger.LogInformation("Deleting doctor with ID: {Id}", id);
            var doctor = await unitOfWork.DoctorRepository.GetByIdAsync(id);
            if (doctor == null)
            {
                logger.LogWarning("Doctor with ID {Id} not found for deletion.", id);
                return NotFound($"Doctor with ID {id} not found.");
            }

            unitOfWork.DoctorRepository.Delete(doctor);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Doctor with ID {Id} deleted successfully.", id);
            return NoContent();
        }
    }
}
