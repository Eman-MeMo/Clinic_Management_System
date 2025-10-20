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
        private readonly IMediator mediator;

        public DoctorController(IDoctorService _doctorService, IMapper _mapper,IMediator _mediator)
        {
            doctorService = _doctorService;
            mapper = _mapper;
            mediator = _mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await doctorService.GetAllAsync();
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
            var doctor = await doctorService.GetByIdAsync(id);
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
            var doctor = await doctorService.GetByEmailAsync(email);
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
            if (doctorDto == null || doctorDto.Id != id)
            {
                return BadRequest("Invalid data!");
            }
            var doctor = await doctorService.GetByIdAsync(id);
            if (doctor == null)
            {
                return NotFound($"Doctor with ID {id} not found.");
            }

            mapper.Map(doctorDto, doctor);
            doctorService.Update(doctor);

            var resultDto = mapper.Map<DoctorDto>(doctor);
            return Ok(resultDto);
        }

        [HttpPut("Deactivate/{id}")]
        public async Task<IActionResult> DeactivateDoctor(string id)
        {
            var doctor = await doctorService.GetByIdAsync(id);
            if (doctor == null)
            {
                return NotFound($"Doctor with ID {id} not found.");
            }

            await mediator.Send(new DeactivateUserCommand { UserId = id, UserType = "Doctor" });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            var doctor = await doctorService.GetByIdAsync(id);
            if (doctor == null)
            {
                return NotFound($"Doctor with ID {id} not found.");
            }

            await doctorService.Delete(doctor);
            return NoContent();
        }
    }
}
