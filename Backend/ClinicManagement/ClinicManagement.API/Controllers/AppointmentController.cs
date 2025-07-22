using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.Enums;
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

        public AppointmentController(IUnitOfWork _unitOfWork, IMapper _mapper, ILogger<AppointmentController> _logger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            logger = _logger;
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

            var appointments = await unitOfWork.AppointmentRepository.GetAllByDoctorIdAsync(id);
            if (appointments == null || !appointments.Any())
            {
                logger.LogWarning("No appointments found for doctor with ID {DoctorId}", id);
                return NotFound("No appointments found for this doctor.");
            }

            logger.LogInformation("Found {Count} appointments for doctor with ID {DoctorId}", appointments.Count(), id);
            var appointmentDtos = mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            return Ok(appointmentDtos);
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

            var appointments = await unitOfWork.AppointmentRepository.GetAllByPatientIdAsync(id);
            if (appointments == null || !appointments.Any())
            {
                logger.LogWarning("No appointments found for patient with ID {PatientId}", id);
                return NotFound("No appointments found for this patient.");
            }

            logger.LogInformation("Found {Count} appointments for patient with ID {PatientId}", appointments.Count(), id);
            var appointmentDtos = mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            return Ok(appointmentDtos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto appointmentDto)
        {
            if (appointmentDto == null)
            {
                logger.LogWarning("CreateAppointment called with null data.");
                return BadRequest("Appointment data is null.");
            }

            logger.LogInformation("Creating appointment for DoctorID {DoctorId} on {Date}", appointmentDto.DoctorId, appointmentDto.Date);

            var isDoctorAvailable = await unitOfWork.AppointmentRepository.IsDoctorAvailableAsync(appointmentDto.DoctorId, appointmentDto.Date);
            if (!isDoctorAvailable)
            {
                logger.LogWarning("Doctor with ID {DoctorId} is not available on {Date}", appointmentDto.DoctorId, appointmentDto.Date);
                return BadRequest("The doctor is not available at the requested time.");
            }

            var appointment = mapper.Map<Domain.Entities.Appointment>(appointmentDto);
            await unitOfWork.AppointmentRepository.AddAsync(appointment);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Appointment with ID {AppointmentId} created successfully", appointment.Id);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, mapper.Map<AppointmentDto>(appointment));
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

            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                logger.LogWarning("Appointment with ID {AppointmentId} not found for cancellation", appointmentId);
                return NotFound("Appointment not found.");
            }

            bool hasSession = await unitOfWork.SessionRepository.HasSessionForAppointmentAsync(appointmentId);
            if (hasSession)
            {
                logger.LogWarning("Cannot cancel appointment with ID {AppointmentId} as it already has an associated session", appointmentId);
                return BadRequest("Cannot cancel appointment. A session has already been started for this appointment.");
            }

            await unitOfWork.AppointmentRepository.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatus.Cancelled);
            logger.LogInformation("Appointment with ID {AppointmentId} was cancelled successfully", appointmentId);

            return Ok("Appointment canceled successfully.");
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDto appointmentDto)
        {
            logger.LogInformation("Updating appointment with ID {AppointmentId}", id);

            if (appointmentDto == null)
            {
                logger.LogWarning("UpdateAppointment called with null data for ID {AppointmentId}", id);
                return BadRequest("Appointment data is null.");
            }

            var existingAppointment = await unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (existingAppointment == null)
            {
                logger.LogWarning("Appointment with ID {AppointmentId} not found for update", id);
                return NotFound($"Appointment with ID {id} not found.");
            }

            var isDoctorAvailable = await unitOfWork.AppointmentRepository.IsDoctorAvailableAsync(appointmentDto.DoctorId, appointmentDto.Date, id);
            if (!isDoctorAvailable)
            {
                logger.LogWarning("Doctor with ID {DoctorId} is not available on {Date} for appointment ID {AppointmentId}", appointmentDto.DoctorId, appointmentDto.Date, id);
                return BadRequest("The doctor is not available at the requested time.");
            }

            mapper.Map(appointmentDto, existingAppointment);
            unitOfWork.AppointmentRepository.Update(existingAppointment);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Appointment with ID {AppointmentId} updated successfully", id);
            return NoContent();
        }

        [HttpPut("status/{id:int}")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] AppointmentStatus status)
        {
            logger.LogInformation("Updating status of appointment with ID {AppointmentId} to {Status}", id, status);

            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                logger.LogWarning("Appointment with ID {AppointmentId} not found for status update", id);
                return NotFound($"Appointment with ID {id} not found.");
            }

            await unitOfWork.AppointmentRepository.UpdateAppointmentStatusAsync(id, status);
            await unitOfWork.SaveChangesAsync();

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
