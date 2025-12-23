using AutoMapper;
using ClinicManagement.Application.Commands.Appointments.BookAppointment;
using ClinicManagement.Application.Commands.Appointments.CancelAppointment;
using ClinicManagement.Application.Commands.Appointments.UpdateAppointment;
using ClinicManagement.Application.Commands.Appointments.UpdateAppointmentStatus;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.Appointments.GetAppointmentsByDoctor;
using ClinicManagement.Application.Queries.Appointments.GetAppointmentsByPatient;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Patient,Doctor")]
    public class AppointmentController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public AppointmentController(IUnitOfWork _unitOfWork, IMapper _mapper, IMediator _mediator)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            mediator = _mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var appointments = unitOfWork.AppointmentRepository.GetAllAsQueryable();
            var totalCount = await appointments.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await appointments.Skip(paginationSkip).Take(pageSize).ToListAsync();

            var appointmentDtos = mapper.Map<List<AppointmentDto>>(items);
            var result = new PaginatedResultDto<AppointmentDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = appointmentDtos
            };
            return Ok(result);
        }

        [HttpGet("doctor/{id}")]
        public async Task<IActionResult> GetAllAppointmentsByDoctorId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Doctor ID is required.");
            }
            var appointments=await mediator.Send(new GetAppointmentsByDoctorQuery { DoctorId = id });
            return Ok(appointments);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                return NotFound($"Appointment with ID {id} not found.");
            }
            var appointmentDto = mapper.Map<AppointmentDto>(appointment);
            return Ok(appointmentDto);
        }

        [HttpGet("patient/{id}")]
        public async Task<IActionResult> GetAllAppointmentsByPatientId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Patient ID is required.");
            }

            var appointments=await mediator.Send(new GetAppointmentsByPatientQuery { PatientId = id });
            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] BookAppointmentCommand command)
        {
            if (command == null)
            {
                return BadRequest("Appointment data is null.");
            }
            var appointmentId = await mediator.Send(command);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointmentId }, appointmentId);
        }

        [HttpPut("cancel-appointment/{appointmentId}")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            if (appointmentId < 1)
            {
                return BadRequest("Invalid appointment ID.");
            }

            await mediator.Send(new CancelAppointmentCommand { Id = appointmentId });
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] UpdateAppointmentCommand command)
        {
            if (command == null || id != command.Id)
            {
                return BadRequest("Invalid appointment data.");
            }
            await mediator.Send(command);
            return NoContent();
        }

        [HttpPut("status/{id:int}")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, AppointmentStatus status)
        {
            await mediator.Send(new UpdateAppointmentStatusCommand { AppointmentId = id, Status = status });
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                return NotFound($"Appointment with ID {id} not found.");
            }

            unitOfWork.AppointmentRepository.Delete(appointment);
            await unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
}
