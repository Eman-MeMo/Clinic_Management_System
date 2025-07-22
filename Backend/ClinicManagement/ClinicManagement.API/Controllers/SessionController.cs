using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.DTOs.SessionDTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<SessionController> logger;

        public SessionController(IUnitOfWork _unitOfWork, IMapper _mapper, ILogger<SessionController> _logger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            logger = _logger;
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetSessionsByDoctor(string doctorId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("Retrieving sessions for doctor {DoctorId}, page {PageNumber}, size {PageSize}", doctorId, pageNumber, pageSize);

            if (string.IsNullOrWhiteSpace(doctorId))
            {
                logger.LogWarning("Doctor ID is null or empty.");
                return BadRequest("Doctor ID cannot be null or empty.");
            }

            var sessions = unitOfWork.SessionRepository.GetSessionsByDoctorAsAsQueryable(doctorId);
            var totalCount = await sessions.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await sessions
                .Skip(paginationSkip)
                .Take(pageSize)
                .ToListAsync();

            var attendanceDtos = mapper.Map<List<AttendanceDto>>(items);
            var result = new PaginatedResultDto<AttendanceDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = attendanceDtos
            };

            logger.LogInformation("Returned {Count} sessions for doctor {DoctorId}", attendanceDtos.Count, doctorId);
            return Ok(result);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetSessionsByPatient(string patientId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("Retrieving sessions for patient {PatientId}, page {PageNumber}, size {PageSize}", patientId, pageNumber, pageSize);

            if (string.IsNullOrWhiteSpace(patientId))
            {
                logger.LogWarning("Patient ID is null or empty.");
                return BadRequest("Patient ID cannot be null or empty.");
            }

            var sessions = unitOfWork.SessionRepository.GetSessionsByPatientAsQueryable(patientId);
            var totalCount = await sessions.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await sessions
                .Skip(paginationSkip)
                .Take(pageSize)
                .ToListAsync();

            var attendanceDtos = mapper.Map<List<AttendanceDto>>(items);
            var result = new PaginatedResultDto<AttendanceDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = attendanceDtos
            };

            logger.LogInformation("Returned {Count} sessions for patient {PatientId}", attendanceDtos.Count, patientId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSessionById(int id)
        {
            logger.LogInformation("Getting session by ID: {SessionId}", id);

            if (id < 1)
            {
                logger.LogWarning("Invalid session ID provided: {SessionId}", id);
                return BadRequest("Session ID cannot be null or empty.");
            }

            var session = await unitOfWork.SessionRepository.GetByIdAsync(id);
            if (session == null)
            {
                logger.LogWarning("Session not found with ID: {SessionId}", id);
                return NotFound("Session not found.");
            }

            var sessionDto = mapper.Map<SessionDto>(session);
            logger.LogInformation("Session with ID {SessionId} retrieved successfully.", id);
            return Ok(sessionDto);
        }

        [HttpPost("start/{appointmentId}")]
        public async Task<IActionResult> StartSession(int appointmentId)
        {
            logger.LogInformation("Starting session for appointment ID: {AppointmentId}", appointmentId);

            if (appointmentId < 1)
            {
                logger.LogWarning("Invalid appointment ID: {AppointmentId}", appointmentId);
                return BadRequest("Appointment ID invalid");
            }

            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                logger.LogWarning("Appointment not found: {AppointmentId}", appointmentId);
                return NotFound("Appointment not found.");
            }

            var result = await unitOfWork.SessionRepository.HasSessionForAppointmentAsync(appointmentId);
            if (result)
            {
                logger.LogWarning("Session already exists for appointment ID: {AppointmentId}", appointmentId);
                return BadRequest($"A session already exists for appointment ID {appointmentId}.");
            }

            var allowedStartTime = appointment.Date.AddMinutes(-10);
            if (DateTime.Now < allowedStartTime)
            {
                logger.LogWarning("Attempt to start session before allowed time for appointment ID: {AppointmentId}", appointmentId);
                return BadRequest("Cannot start session before the scheduled time.");
            }

            if (appointment.Status != AppointmentStatus.Confirmed)
            {
                logger.LogWarning("Appointment {AppointmentId} not confirmed. Status: {Status}", appointmentId, appointment.Status);
                return BadRequest("Appointment must be confirmed before starting a session.");
            }

            var sessionId = await unitOfWork.SessionRepository.CreateSessionAsync(appointmentId);
            logger.LogInformation("Session started successfully for appointment ID {AppointmentId}. Session ID: {SessionId}", appointmentId, sessionId);

            return CreatedAtAction(nameof(GetSessionById), new { id = sessionId }, $"Session started for appointment ID {appointmentId} with id {sessionId}.");
        }

        [HttpPut("end/{sessionId}")]
        public async Task<IActionResult> EndSession(int sessionId, [FromQuery] string status)
        {
            logger.LogInformation("Ending session ID: {SessionId} with status: {Status}", sessionId, status);

            if (sessionId < 1)
            {
                logger.LogWarning("Invalid session ID: {SessionId}", sessionId);
                return BadRequest("Invalid session ID.");
            }

            var session = await unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if (session == null)
            {
                logger.LogWarning("Session not found: {SessionId}", sessionId);
                return NotFound("Session not found.");
            }

            if (!Enum.TryParse<SessionStatus>(status, true, out var sessionStatus))
            {
                logger.LogWarning("Invalid session status received: {Status}", status);
                return BadRequest("Invalid session status. Allowed: Completed or Canceled.");
            }

            await unitOfWork.SessionRepository.EndSession(sessionId, sessionStatus);
            logger.LogInformation("Session ID {SessionId} ended with status: {Status}", sessionId, sessionStatus);

            return Ok($"Session ended with status {status}.");
        }

        [HttpPut("add-doctor-notes/{sessionId}")]
        public async Task<IActionResult> AddDoctorNotes(int sessionId, string notes)
        {
            logger.LogInformation("Adding doctor notes to session ID: {SessionId}", sessionId);

            if (sessionId < 1 || string.IsNullOrWhiteSpace(notes))
            {
                logger.LogWarning("Invalid data for adding doctor notes. Session ID: {SessionId}", sessionId);
                return BadRequest("invalid data");
            }

            var session = await unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if (session == null)
            {
                logger.LogWarning("Session not found for adding notes. Session ID: {SessionId}", sessionId);
                return NotFound("Session not found.");
            }

            await unitOfWork.SessionRepository.AddDoctorNotes(sessionId, notes);
            logger.LogInformation("Doctor notes added successfully for session ID: {SessionId}", sessionId);

            return Ok("Doctor notes added successfully.");
        }
    }
}
