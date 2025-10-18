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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<AppointmentController> logger;
        private readonly IMediator mediator;

        public AppointmentController(IUnitOfWork _unitOfWork, IMapper _mapper, ILogger<AppointmentController> _logger, IMediator _mediator)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            logger = _logger;
            mediator = _mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("Fetching all appointments with pagination: page {PageNumber}, size {PageSize}", pageNumber, pageSize);

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

            logger.LogInformation("Returned {Count} appointments out of {TotalCount}", appointmentDtos.Count, totalCount);
            return Ok(result);
        }

        [HttpGet("doctor/{id}")]
        public async Task<IActionResult> GetAllAppointmentsByDoctorId(string id)
        {
            logger.LogInformation("Fetching all appointments for doctor with ID {DoctorId}", id);

            if (string.IsNullOrWhiteSpace(id))
            {
                logger.LogWarning("GetAllAppointmentsByDoctorId called with null or empty doctor ID.");
                return BadRequest("Doctor ID is required.");
            }
            var appointments=await mediator.Send(new GetAppointmentsByDoctorQuery { DoctorId = id });

            logger.LogInformation("Found {Count} appointments for doctor with ID {DoctorId}", appointments.Count(), id);
            return Ok(appointments);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            logger.LogInformation("Fetching appointment with ID {AppointmentId}", id);

            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                logger.LogWarning("Appointment with ID {AppointmentId} not found", id);
                return NotFound($"Appointment with ID {id} not found.");
            }

            logger.LogInformation("Appointment with ID {AppointmentId} retrieved successfully", id);
            var appointmentDto = mapper.Map<AppointmentDto>(appointment);
            return Ok(appointmentDto);
        }

        [HttpGet("patient/{id}")]
        public async Task<IActionResult> GetAllAppointmentsByPatientId(string id)
        {
            logger.LogInformation("Fetching all appointments for patient with ID {PatientId}", id);

            if (string.IsNullOrWhiteSpace(id))
            {
                logger.LogWarning("GetAllAppointmentsByPatientId called with null or empty patient ID.");
                return BadRequest("Patient ID is required.");
            }

            var appointments=await mediator.Send(new GetAppointmentsByPatientQuery { PatientId = id });

            logger.LogInformation("Found {Count} appointments for patient with ID {PatientId}", appointments.Count(), id);
            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] BookAppointmentCommand command)
        {
            if (command == null)
            {
                logger.LogWarning("CreateAppointment called with null data.");
                return BadRequest("Appointment data is null.");
            }

            logger.LogInformation("Creating appointment for DoctorID {DoctorId} on {Date}", command.DoctorId, command.Date);

            var appointmentId = await mediator.Send(command);

            logger.LogInformation("Appointment with ID {AppointmentId} created successfully", appointmentId);

            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointmentId }, appointmentId);
        }

        [HttpPut("cancel-appointment/{appointmentId}")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            logger.LogInformation("Attempting to cancel appointment with ID {AppointmentId}", appointmentId);

            if (appointmentId < 1)
            {
                logger.LogWarning("Invalid appointment ID {AppointmentId} provided for cancellation", appointmentId);
                return BadRequest("Invalid appointment ID.");
            }

            await mediator.Send(new CancelAppointmentCommand { Id = appointmentId });
            logger.LogInformation("Appointment with ID {AppointmentId} was cancelled successfully", appointmentId);

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] UpdateAppointmentCommand command)
        {
            if (command == null || id != command.Id)
            {
                logger.LogWarning("UpdateAppointment called with invalid data for ID {AppointmentId}", id);
                return BadRequest("Invalid appointment data.");
            }

            logger.LogInformation("Updating appointment with ID {AppointmentId}", id);

            await mediator.Send(command);

            logger.LogInformation("Appointment with ID {AppointmentId} updated successfully", id);
            return NoContent();
        }

        [HttpPut("status/{id:int}")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] AppointmentStatus status)
        {
            logger.LogInformation("Updating status of appointment with ID {AppointmentId} to {Status}", id, status);

            await mediator.Send(new UpdateAppointmentStatusCommand { AppointmentId = id, Status = status });

            logger.LogInformation("Appointment status updated successfully for ID {AppointmentId}", id);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            logger.LogInformation("Deleting appointment with ID {AppointmentId}", id);

            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                logger.LogWarning("Appointment with ID {AppointmentId} not found for deletion", id);
                return NotFound($"Appointment with ID {id} not found.");
            }

            unitOfWork.AppointmentRepository.Delete(appointment);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Appointment with ID {AppointmentId} deleted successfully", id);
            return NoContent();
        }
    }
}
