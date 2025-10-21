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
        private readonly IMediator mediator;

        public PatientController(IPatientService _patientService, IMapper _mapper, IMediator _mediator)
        {
            patientService = _patientService;
            mapper = _mapper;
            mediator = _mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPatients([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var patients =  patientService.GetAllAsQueryable();
            var totalCount = await patients.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await patients.Skip(paginationSkip).Take(pageSize).ToListAsync();

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
            var patient = await patientService.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound($"Patient with ID {id} not found.");
            }

            return Ok(mapper.Map<PatientDto>(patient));
        }

        [HttpGet("NationalId/{nationalId}")]
        public async Task<IActionResult> GetPatientByNationalId(string nationalId)
        {
            var patient = await patientService.GetByNationalIdAsync(nationalId);
            if (patient == null)
            {
                return NotFound($"Patient with National ID {nationalId} not found.");
            }

            return Ok(mapper.Map<PatientDto>(patient));
        }

        [HttpGet("Email/{email}")]
        public async Task<IActionResult> GetPatientByEmail(string email)
        {
            var patient = await patientService.GetByEmailAsync(email);
            if (patient == null)
            {
                return NotFound($"Patient with Email {email} not found.");
            }

            return Ok(mapper.Map<PatientDto>(patient));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(string id,[FromBody] PatientDto patientDto)
        {
            if (patientDto == null || patientDto.Id!=id)
            {
                return BadRequest("Invalid patient data.");
            }

            var patient = await patientService.GetByIdAsync(patientDto.Id);
            if (patient == null)
            {
                return NotFound($"Patient with ID {patientDto.Id} not found.");
            }

            mapper.Map(patientDto, patient);
            await patientService.Update(patient);
            return NoContent();
        }

        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivatePatient(string id)
        {
            var patient = await patientService.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound($"Patient with ID {id} not found.");
            }

            await mediator.Send(new DeactivateUserCommand { UserId = id, UserType = "Patient" });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(string id)
        {
            var patient = await patientService.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound($"Patient with ID {id} not found.");
            }

            await patientService.Delete(patient);
            return NoContent();
        }
    }
}
