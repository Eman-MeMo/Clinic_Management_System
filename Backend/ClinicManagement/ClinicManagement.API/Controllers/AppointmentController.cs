using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await unitOfWork.AppointmentRepository.GetAllAsync();
            if (appointments == null || !appointments.Any())
            {
                return NotFound("No appointments found.");
            }
            var appointmentDtos = mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            return Ok(appointmentDtos);
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDto appointmentDto) 
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
