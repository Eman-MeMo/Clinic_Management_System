using AutoMapper;
using ClinicManagement.Application.Commands.Sessions.EndSession;
using ClinicManagement.Application.Commands.Sessions.StartSession;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.DTOs.SessionDTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using MediatR;
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
        private readonly IMediator mediator;


        public SessionController(IUnitOfWork _unitOfWork, IMapper _mapper, ILogger<SessionController> _logger, IMediator _mediator)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            logger = _logger;
            mediator = _mediator;
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
            var sessionId = await mediator.Send(new StartSessionCommand { AppointmentId = appointmentId });
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

            if (!Enum.TryParse<SessionStatus>(status, true, out var parsedStatus))
            {
                logger.LogWarning("Invalid session status value: {Status}", status);
                return BadRequest("Invalid status value.");
            }

            await mediator.Send(new EndSessionCommand { SessionId = sessionId, Status = parsedStatus });

            logger.LogInformation("Session ID {SessionId} ended with status: {Status}", sessionId, status);

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
