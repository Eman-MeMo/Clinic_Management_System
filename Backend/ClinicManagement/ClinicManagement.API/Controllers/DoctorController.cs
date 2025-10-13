using AutoMapper;
using ClinicManagement.Application.Commands.Users.DeactivateUser;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.DTOs.DoctorDTOs;
using ClinicManagement.Domain.Entities;
using MediatR;
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
        private readonly IDoctorService doctorService;
        private readonly IMapper mapper;
        private readonly ILogger<DoctorController> logger;
        private readonly IMediator mediator;

        public DoctorController(IDoctorService _doctorService, IMapper _mapper, ILogger<DoctorController> _logger, IMediator _mediator)
        {
            doctorService = _doctorService;
            mapper = _mapper;
            logger = _logger;
            mediator = _mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            logger.LogInformation("Getting all doctors.");
            var doctors = await doctorService.GetAllAsync();
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
            var doctor = await doctorService.GetByIdAsync(id);
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
            var doctor = await doctorService.GetByEmailAsync(email);
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
            var doctor = await doctorService.GetByIdAsync(id);
            if (doctor == null)
            {
                logger.LogWarning("Doctor with ID {Id} not found for update.", id);
                return NotFound($"Doctor with ID {id} not found.");
            }

            mapper.Map(doctorDto, doctor);
            doctorService.Update(doctor);

            logger.LogInformation("Doctor with ID {Id} updated successfully.", id);
            var resultDto = mapper.Map<DoctorDto>(doctor);
            return Ok(resultDto);
        }

        [HttpPut("Deactivate/{id}")]
        public async Task<IActionResult> DeactivateDoctor(string id)
        {
            logger.LogInformation("Deactivating doctor with ID: {Id}", id);
            var doctor = await doctorService.GetByIdAsync(id);
            if (doctor == null)
            {
                logger.LogWarning("Doctor with ID {Id} not found for deactivation.", id);
                return NotFound($"Doctor with ID {id} not found.");
            }

            await mediator.Send(new DeactivateUserCommand { UserId = id, UserType = "Doctor" });

            logger.LogInformation("Doctor with ID {Id} deactivated successfully.", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            logger.LogInformation("Deleting doctor with ID: {Id}", id);
            var doctor = await doctorService.GetByIdAsync(id);
            if (doctor == null)
            {
                logger.LogWarning("Doctor with ID {Id} not found for deletion.", id);
                return NotFound($"Doctor with ID {id} not found.");
            }

            await doctorService.Delete(doctor);

            logger.LogInformation("Doctor with ID {Id} deleted successfully.", id);
            return NoContent();
        }
    }
}
