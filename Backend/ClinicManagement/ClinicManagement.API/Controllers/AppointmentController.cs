using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.DTOs.PatientDTOs;
using ClinicManagement.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuditLoggerService auditLogger;
        private string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public AppointmentController(IUnitOfWork _unitOfWork, IMapper _mapper, IAuditLoggerService _auditLogger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            auditLogger = _auditLogger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAppointments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var appointments =  unitOfWork.AppointmentRepository.GetAllAsQueryable();
            var totalCount = await appointments.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await appointments
                .Skip(paginationSkip)
                .Take(pageSize)
                .ToListAsync();

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
            var appointments = await unitOfWork.AppointmentRepository.GetAllByDoctorIdAsync(id);
            if (appointments == null || !appointments.Any())
            {
                return NotFound("No appointments found for this doctor.");
            }
            var appointmentDtos = mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            return Ok(appointmentDtos);
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
            var appointments = await unitOfWork.AppointmentRepository.GetAllByPatientIdAsync(id);
            if (appointments == null || !appointments.Any())
            {
                return NotFound("No appointments found for this patient.");
            }
            var appointmentDtos = mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            return Ok(appointmentDtos);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto appointmentDto)
        {
            if (appointmentDto == null)
            {
                return BadRequest("Appointment data is null.");
            }
            // Check if the doctor is available for the requested appointment time
            var isDoctorAvailable = await unitOfWork.AppointmentRepository.IsDoctorAvailableAsync(appointmentDto.DoctorId, appointmentDto.Date);
            if (!isDoctorAvailable)
            {
                return BadRequest("The doctor is not available at the requested time.");
            }

            var appointment = mapper.Map<Domain.Entities.Appointment>(appointmentDto);
            await unitOfWork.AppointmentRepository.AddAsync(appointment);
            await unitOfWork.SaveChangesAsync();

            // Log the creation of the appointment
            await auditLogger.LogAsync("Create", "Appointment", $"Created appointment with ID {appointment.Id}", UserId);

            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, mapper.Map<AppointmentDto>(appointment));
        }
        [HttpPut("cancel-appointment/{appointmentId}")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            if (appointmentId < 1)
                return BadRequest("Invalid appointment ID.");

            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null)
                return NotFound("Appointment not found.");

            bool hasSession = await unitOfWork.SessionRepository.HasSessionForAppointmentAsync(appointmentId);
            if (hasSession)
                return BadRequest("Cannot cancel appointment. A session has already been started for this appointment.");

            await unitOfWork.AppointmentRepository.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatus.Cancelled);

            // Log the cancellation of the appointment
            await auditLogger.LogAsync("Cancel Appointment", "Appointment", $"Canceled appointment ID {appointmentId}", UserId);
            return Ok("Appointment canceled successfully.");
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDto appointmentDto) 
        {
            if (appointmentDto == null)
            {
                return BadRequest("Appointment data is null.");
            }
            var existingAppointment = await unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (existingAppointment == null)
            {
                return NotFound($"Appointment with ID {id} not found.");
            }
            // Check if the doctor is available for the updated appointment time
            var isDoctorAvailable = await unitOfWork.AppointmentRepository.IsDoctorAvailableAsync(appointmentDto.DoctorId, appointmentDto.Date, id);
            if (!isDoctorAvailable)
            {
                return BadRequest("The doctor is not available at the requested time.");
            }

            mapper.Map(appointmentDto, existingAppointment);
            unitOfWork.AppointmentRepository.Update(existingAppointment);
            await unitOfWork.SaveChangesAsync();

            // Log the update of the appointment
            await auditLogger.LogAsync("Update", "Appointment", $"Updated appointment with ID {existingAppointment.Id}", UserId);
            return NoContent();
        }
        [HttpPut("status/{id:int}")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] AppointmentStatus status)
        {
            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                return NotFound($"Appointment with ID {id} not found.");
            }
            // Update the appointment status
            appointment.Status = status;

            await unitOfWork.AppointmentRepository.UpdateAppointmentStatusAsync(id, status);
            await unitOfWork.SaveChangesAsync();

            // Log the status update of the appointment
            await auditLogger.LogAsync("Update", "Appointment Status", $"Updated status of appointment with ID {appointment.Id} to {status}", UserId);
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

            // Log the deletion of the appointment
            await auditLogger.LogAsync("Delete", "Appointment", $"Deleted appointment with ID {appointment.Id}", UserId);
            return NoContent();
        }
    }
}
