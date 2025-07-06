using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.PatientDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuditLoggerService auditLogger;
        private string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public PatientController(IUnitOfWork _unitOfWork, IMapper _mapper, IAuditLoggerService _auditLogger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            auditLogger = _auditLogger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await unitOfWork.PatientRepository.GetAllAsync();
            if (patients == null || !patients.Any())
            {
                return NotFound("No patients found.");
            }
            var patientDtos = mapper.Map<IEnumerable<PatientDto>>(patients);
            return Ok(patientDtos);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(string id)
        {
            var patient = await unitOfWork.PatientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound($"Patient with ID {id} not found.");
            }
            var patientDto = mapper.Map<PatientDto>(patient);
            return Ok(patientDto);
        }
        [HttpGet("NationalId/{nationalId}")]
        public async Task<IActionResult> GetPatientByNationalId(string nationalId)
        {
            var patient = await unitOfWork.PatientRepository.GetByNationalIdAsync(nationalId);
            if (patient == null)
            {
                return NotFound($"Patient with National ID {nationalId} not found.");
            }
            var patientDto = mapper.Map<PatientDto>(patient);
            return Ok(patientDto);
        }
        [HttpGet("Email/{email}")]
        public async Task<IActionResult> GetPatientByEmail(string email)
        {
            var patient = await unitOfWork.PatientRepository.GetByEmailAsync(email);
            if (patient == null)
            {
                return NotFound($"Patient with Email {email} not found.");
            }
            var patientDto = mapper.Map<PatientDto>(patient);
            return Ok(patientDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient([FromBody] PatientDto patientDto)
        {
            if (patientDto == null || string.IsNullOrEmpty(patientDto.Id))
            {
                return BadRequest("Invalid patient data.");
            }
            var patient = await unitOfWork.PatientRepository.GetByIdAsync(patientDto.Id);
            if (patient == null)
            {
                return NotFound($"Patient with ID {patientDto.Id} not found.");
            }
            mapper.Map(patientDto, patient);
            unitOfWork.PatientRepository.Update(patient);
            await unitOfWork.SaveChangesAsync();

            // Log the update action
            await auditLogger.LogAsync("Update", "Patients", $"Updated patient with ID {patientDto.Id}", UserId);

            return NoContent();
        }
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivatePatient(string id)
        {
            var patient = await unitOfWork.PatientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound($"Patient with ID {id} not found.");
            }
            await unitOfWork.PatientRepository.DeactivateUserAsync(id);
            await unitOfWork.SaveChangesAsync();

            // Log the deactivation action
            await auditLogger.LogAsync("Deactivate", "Patients", $"Deactivated patient with ID {id}", UserId);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(string id)
        {
            var patient = await unitOfWork.PatientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound($"Patient with ID {id} not found.");
            }
            unitOfWork.PatientRepository.Delete(patient);
            await unitOfWork.SaveChangesAsync();

            // Log the deletion action
            await auditLogger.LogAsync("Delete", "Patients", $"Deleted patient with ID {id}", UserId);
            return NoContent();
        }
    }
}
