using AutoMapper;
using ClinicManagement.Application.Commands.Users.DeactivateUser;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.DTOs.PatientDTOs;
using ClinicManagement.Domain.Entities;
using MediatR;
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
        private readonly IPatientService patientService;
        private readonly IMapper mapper;
        private readonly ILogger<PatientController> logger;
        private readonly IMediator mediator;

        public PatientController(IPatientService _patientService, IMapper _mapper, ILogger<PatientController> _logger, IMediator _mediator)
        {
            patientService = _patientService;
            mapper = _mapper;
            logger = _logger;
            mediator = _mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPatients([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("Fetching patients: Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);

            var patients =  patientService.GetAllAsQueryable();
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

            var patient = await patientService.GetByIdAsync(id);
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

            var patient = await patientService.GetByNationalIdAsync(nationalId);
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

            var patient = await patientService.GetByEmailAsync(email);
            if (patient == null)
            {
                logger.LogWarning("Patient with Email {Email} not found.", email);
                return NotFound($"Patient with Email {email} not found.");
            }

            return Ok(mapper.Map<PatientDto>(patient));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(string id,[FromBody] PatientDto patientDto)
        {
            if (patientDto == null || patientDto.Id!=id)
            {
                logger.LogWarning("Invalid patient data for update.");
                return BadRequest("Invalid patient data.");
            }

            logger.LogInformation("Updating patient with ID: {Id}", patientDto.Id);

            var patient = await patientService.GetByIdAsync(patientDto.Id);
            if (patient == null)
            {
                logger.LogWarning("Patient with ID {Id} not found for update.", patientDto.Id);
                return NotFound($"Patient with ID {patientDto.Id} not found.");
            }

            mapper.Map(patientDto, patient);
            await patientService.Update(patient);

            logger.LogInformation("Patient with ID {Id} updated successfully.", patientDto.Id);

            return NoContent();
        }

        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivatePatient(string id)
        {
            logger.LogInformation("Deactivating patient with ID: {Id}", id);

            var patient = await patientService.GetByIdAsync(id);
            if (patient == null)
            {
                logger.LogWarning("Patient with ID {Id} not found for deactivation.", id);
                return NotFound($"Patient with ID {id} not found.");
            }

            await mediator.Send(new DeactivateUserCommand { UserId = id, UserType = "Patient" });

            logger.LogInformation("Patient with ID {Id} deactivated successfully.", id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(string id)
        {
            logger.LogInformation("Deleting patient with ID: {Id}", id);

            var patient = await patientService.GetByIdAsync(id);
            if (patient == null)
            {
                logger.LogWarning("Patient with ID {Id} not found for deletion.", id);
                return NotFound($"Patient with ID {id} not found.");
            }

            await patientService.Delete(patient);

            logger.LogInformation("Patient with ID {Id} deleted successfully.", id);

            return NoContent();
        }
    }
}
