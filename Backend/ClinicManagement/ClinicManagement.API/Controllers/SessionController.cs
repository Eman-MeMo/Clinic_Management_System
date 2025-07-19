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
using System.Drawing.Printing;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuditLoggerService auditLogger;
        private string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public SessionController(IUnitOfWork _unitOfWork, IMapper _mapper, IAuditLoggerService _auditLogger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            auditLogger = _auditLogger;
        }
        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetSessionsByDoctor(string doctorId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(doctorId))
                return BadRequest("Doctor ID cannot be null or empty.");

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
            return Ok(result);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetSessionsByPatient(string patientId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(patientId))
                return BadRequest("Patient ID cannot be null or empty.");
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
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSessionById(int id)
        {
            if (id<1)
                return BadRequest("Session ID cannot be null or empty.");
            var session = await unitOfWork.SessionRepository.GetByIdAsync(id);
            if (session == null)
                return NotFound("Session not found.");
            var sessionDto = mapper.Map<SessionDto>(session);
            return Ok(sessionDto);
        }
        [HttpPost("start/{appointmentId}")]
        public async Task<IActionResult> StartSession(int appointmentId)
        {
            if (appointmentId<1)
                return BadRequest("Appointment ID invalid");

            // Check if the appointment exists and is not already in session
            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null)
                return NotFound("Appointment not found.");

            var result = await unitOfWork.SessionRepository.HasSessionForAppointmentAsync(appointmentId);
            if (result)
                return BadRequest($"A session already exists for appointment ID {appointmentId}.");

            var allowedStartTime = appointment.Date.AddMinutes(-10);  // Allow 10 mins before
            if (DateTime.Now < allowedStartTime)
                return BadRequest("Cannot start session before the scheduled time.");


            // Check if the appointment is confirmed
            if (appointment.Status != Domain.Enums.AppointmentStatus.Confirmed)
                return BadRequest("Appointment must be confirmed before starting a session.");

            // Start the session
            var sessionId=await unitOfWork.SessionRepository.CreateSessionAsync(appointmentId);

            // Log the action
            await auditLogger.LogAsync("Start Session", "Session", $"Started session for appointment ID {appointmentId}", UserId);
            return CreatedAtAction(nameof(GetSessionById), new { id = sessionId }, $"Session started for appointment ID {appointmentId} with id {sessionId}.");
        }
        [HttpPut("end/{sessionId}")]
        public async Task<IActionResult> EndSession(int sessionId, [FromQuery] string status)
        {
            if (sessionId < 1)
                return BadRequest("Invalid session ID.");

            var session = await unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if (session == null)
                return NotFound("Session not found.");

            if (!Enum.TryParse<SessionStatus>(status, true, out var sessionStatus))
                return BadRequest("Invalid session status. Allowed: Completed or Canceled.");

            await unitOfWork.SessionRepository.EndSession(sessionId, sessionStatus);

            // Log the action
            await auditLogger.LogAsync("End Session", "Session", $"Ended session ID {sessionId} with status {status}", UserId);

            return Ok($"Session ended with status {status}.");
        }

        [HttpPut("add-doctor-notes/{sessionId}")]
        public async Task<IActionResult> AddDoctorNotes(int sessionId, string notes)
        {
            if (sessionId<1 || string.IsNullOrWhiteSpace(notes))
                return BadRequest("invalid data");

            // Check if the session exists
            var session = await unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if (session == null)
                return NotFound("Session not found.");

            // Add notes to the session
            await unitOfWork.SessionRepository.AddDoctorNotes(sessionId, notes);

            // Log the action
            await auditLogger.LogAsync("Add Doctor Notes", "Session", $"Added notes to session ID {sessionId}", UserId);
            return Ok("Doctor notes added successfully.");
        }

    }
}
