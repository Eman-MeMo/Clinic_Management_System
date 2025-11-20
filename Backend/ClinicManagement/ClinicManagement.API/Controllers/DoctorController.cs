using AutoMapper;
using ClinicManagement.Application.Commands.Users.DeactivateUser;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.DTOs.DoctorDTOs;
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
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService doctorService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public DoctorController(IDoctorService _doctorService,IUnitOfWork _unitOfWork, IMapper _mapper,IMediator _mediator)
        {
            doctorService = _doctorService;
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            mediator = _mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoctors([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var doctors = doctorService.GetAllAsQueryable();
            if (doctors == null || !doctors.Any())
            {
                return NotFound("No doctors found.");
            }
            var totalCount = await doctors.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await doctors.Skip(paginationSkip).Take(pageSize).ToListAsync();

            var result = new PaginatedResultDto<DoctorDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = mapper.Map<List<DoctorDto>>(items)
            };

            return Ok(result);
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
            var specialization = await unitOfWork.SpecializationRepository.GetByIdAsync(doctorDto.SpecializationId);
            if (specialization == null)
            {
                return BadRequest($"Specialization with ID {doctorDto.SpecializationId} does not exist.");
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
