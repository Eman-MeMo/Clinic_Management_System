using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.DTOs.PatientDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<PatientController> logger;

        public PatientController(IUnitOfWork _unitOfWork, IMapper _mapper, ILogger<PatientController> _logger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            logger = _logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPatients([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("Fetching patients: Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);

            var patients = unitOfWork.PatientRepository.GetAllAsQueryable();
            var totalCount = await patients.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await patients.Skip(paginationSkip).Take(pageSize).ToListAsync();

            logger.LogInformation("Fetched {Count} patients.", items.Count);

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
            logger.LogInformation("Fetching patient by ID: {Id}", id);

            var patient = await unitOfWork.PatientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                logger.LogWarning("Patient with ID {Id} not found.", id);
                return NotFound($"Patient with ID {id} not found.");
            }

            return Ok(mapper.Map<PatientDto>(patient));
        }

        [HttpGet("NationalId/{nationalId}")]
        public async Task<IActionResult> GetPatientByNationalId(string nationalId)
        {
            logger.LogInformation("Fetching patient by National ID: {NationalId}", nationalId);

            var patient = await unitOfWork.PatientRepository.GetByNationalIdAsync(nationalId);
            if (patient == null)
            {
                logger.LogWarning("Patient with National ID {NationalId} not found.", nationalId);
                return NotFound($"Patient with National ID {nationalId} not found.");
            }

            return Ok(mapper.Map<PatientDto>(patient));
        }

        [HttpGet("Email/{email}")]
        public async Task<IActionResult> GetPatientByEmail(string email)
        {
            logger.LogInformation("Fetching patient by email: {Email}", email);

            var patient = await unitOfWork.PatientRepository.GetByEmailAsync(email);
            if (patient == null)
            {
                logger.LogWarning("Patient with Email {Email} not found.", email);
                return NotFound($"Patient with Email {email} not found.");
            }

            return Ok(mapper.Map<PatientDto>(patient));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient([FromBody] PatientDto patientDto)
        {
            if (patientDto == null || string.IsNullOrEmpty(patientDto.Id))
            {
                logger.LogWarning("Invalid patient data for update.");
                return BadRequest("Invalid patient data.");
            }

            logger.LogInformation("Updating patient with ID: {Id}", patientDto.Id);

            var patient = await unitOfWork.PatientRepository.GetByIdAsync(patientDto.Id);
            if (patient == null)
            {
                logger.LogWarning("Patient with ID {Id} not found for update.", patientDto.Id);
                return NotFound($"Patient with ID {patientDto.Id} not found.");
            }

            mapper.Map(patientDto, patient);
            unitOfWork.PatientRepository.Update(patient);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Patient with ID {Id} updated successfully.", patientDto.Id);

            return NoContent();
        }

        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivatePatient(string id)
        {
            logger.LogInformation("Deactivating patient with ID: {Id}", id);

            var patient = await unitOfWork.PatientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                logger.LogWarning("Patient with ID {Id} not found for deactivation.", id);
                return NotFound($"Patient with ID {id} not found.");
            }

            await unitOfWork.PatientRepository.DeactivateUserAsync(id);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Patient with ID {Id} deactivated successfully.", id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(string id)
        {
            logger.LogInformation("Deleting patient with ID: {Id}", id);

            var patient = await unitOfWork.PatientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                logger.LogWarning("Patient with ID {Id} not found for deletion.", id);
                return NotFound($"Patient with ID {id} not found.");
            }

            unitOfWork.PatientRepository.Delete(patient);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Patient with ID {Id} deleted successfully.", id);

            return NoContent();
        }
    }
}
