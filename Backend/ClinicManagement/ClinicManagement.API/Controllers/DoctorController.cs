using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.DoctorDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuditLoggerService auditLogger;
        private string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public DoctorController(IUnitOfWork _unitOfWork, IMapper _mapper, IAuditLoggerService _auditLogger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            auditLogger = _auditLogger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await unitOfWork.DoctorRepository.GetAllAsync();
            if (doctors == null || !doctors.Any())
            {
                return NotFound("No doctors found.");
            }
            var doctorDtos = mapper.Map<IEnumerable<DoctorDto>>(doctors);
            return Ok(doctorDtos);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(string id)
        {
            var doctor = await unitOfWork.DoctorRepository.GetByIdAsync(id);
            if (doctor == null)
            {
                return NotFound($"Doctor with ID {id} not found.");
            }
            var doctorDto = mapper.Map<DoctorDto>(doctor);
            return Ok(doctorDto);
        }
        [HttpGet("Email/{email}")]
        public async Task<IActionResult> GetDoctorByEmail(string email)
        {
            var doctor = await unitOfWork.DoctorRepository.GetByEmailAsync(email);
            if (doctor == null)
            {
                return NotFound($"Doctor with Email {email} not found.");
            }
            var doctorDto = mapper.Map<DoctorDto>(doctor);
            return Ok(doctorDto);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(string id, [FromBody] DoctorDto doctorDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (doctorDto == null || doctorDto.Id!=id)
            {
                return BadRequest("Invalid data!");
            }
            var doctor = await unitOfWork.DoctorRepository.GetByIdAsync(id);
            if (doctor == null)
            {
                return NotFound($"Doctor with ID {id} not found.");
            }
            mapper.Map(doctorDto, doctor);
            unitOfWork.DoctorRepository.Update(doctor);
            await unitOfWork.SaveChangesAsync();

            // Log the update of the doctor
            await auditLogger.LogAsync("Update", "Doctor", $"Updated doctor with ID {doctor.Id}", UserId);
            var resultDto = mapper.Map<DoctorDto>(doctor);
            return Ok(resultDto);
        }
        [HttpPut("Deactivate/{id}")]
        public async Task<IActionResult> DeactivateDoctor(string id)
        {
            var doctor = await unitOfWork.DoctorRepository.GetByIdAsync(id);
            if (doctor == null)
            {
                return NotFound($"Doctor with ID {id} not found.");
            }
            await unitOfWork.DoctorRepository.DeactivateUserAsync(id);
            await unitOfWork.SaveChangesAsync();

            // Log the deactivation of the doctor
            await auditLogger.LogAsync("Deactivate", "Doctor", $"Deactivated doctor with ID {id}", UserId);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            var doctor = await unitOfWork.DoctorRepository.GetByIdAsync(id);
            if (doctor == null)
            {
                return NotFound($"Doctor with ID {id} not found.");
            }
            unitOfWork.DoctorRepository.Delete(doctor);
            await unitOfWork.SaveChangesAsync();

            // Log the deletion of the doctor
            await auditLogger.LogAsync("Delete", "Doctor", $"Deleted doctor with ID {doctor.Id}", UserId);
            return NoContent();
        }
    }
}
