using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.DTOs.PatientDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public async Task<IActionResult> GetAllPatients([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var patients = unitOfWork.PatientRepository.GetAllAsQueryable();

            var totalCount = await patients.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;

            var items = await patients
                .Skip(paginationSkip)
                .Take(pageSize)
                .ToListAsync();

            var result = new PaginatedResultDto<PatientDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = mapper.Map<List<PatientDto>>(items)
            };

            return Ok(result);
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
