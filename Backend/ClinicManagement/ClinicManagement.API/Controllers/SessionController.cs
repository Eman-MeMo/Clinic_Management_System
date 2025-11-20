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
        private readonly IMediator mediator;


        public SessionController(IUnitOfWork _unitOfWork, IMapper _mapper, IMediator _mediator)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            mediator = _mediator;
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetSessionsByDoctor(string doctorId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(doctorId))
            {
                return BadRequest("Doctor ID cannot be null or empty.");
            }

            var sessions = unitOfWork.SessionRepository.GetSessionsByDoctorAsAsQueryable(doctorId);
            var totalCount = await sessions.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await sessions
                .Skip(paginationSkip)
                .Take(pageSize)
                .ToListAsync();

            var sessioneDtos = mapper.Map<List<SessionDto>>(items);
            var result = new PaginatedResultDto<SessionDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = sessioneDtos
            };

            return Ok(result);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetSessionsByPatient(string patientId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                return BadRequest("Patient ID cannot be null or empty.");
            }

            var sessions = unitOfWork.SessionRepository.GetSessionsByPatientAsQueryable(patientId);
            var totalCount = await sessions.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await sessions
                .Skip(paginationSkip)
                .Take(pageSize)
                .ToListAsync();

            var sessioneDtos = mapper.Map<List<SessionDto>>(items);
            var result = new PaginatedResultDto<SessionDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = sessioneDtos
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSessionById(int id)
        {
            if (id < 1)
            {
                return BadRequest("Session ID cannot be null or empty.");
            }

            var session = await unitOfWork.SessionRepository.GetByIdAsync(id);
            if (session == null)
            {
                return NotFound("Session not found.");
            }

            var sessionDto = mapper.Map<SessionDto>(session);
            return Ok(sessionDto);
        }

        [HttpPost("start/{appointmentId}")]
        public async Task<IActionResult> StartSession(int appointmentId)
        {
            if (appointmentId < 1)
            {
                return BadRequest("Appointment ID invalid");
            }
            var sessionId = await mediator.Send(new StartSessionCommand { AppointmentId = appointmentId });
            return CreatedAtAction(nameof(GetSessionById), new { id = sessionId }, $"Session started for appointment ID {appointmentId} with id {sessionId}.");
        }

        [HttpPut("end/{sessionId}")]
        public async Task<IActionResult> EndSession(int sessionId, [FromQuery] string status)
        {
            if (sessionId < 1)
            {
                return BadRequest("Invalid session ID.");
            }

            if (!Enum.TryParse<SessionStatus>(status, true, out var parsedStatus))
            {
                return BadRequest("Invalid status value.");
            }

            await mediator.Send(new EndSessionCommand { SessionId = sessionId, Status = parsedStatus });
            return Ok($"Session ended with status {status}.");
        }

        [HttpPut("add-doctor-notes/{sessionId}")]
        public async Task<IActionResult> UpdateDoctorNotes(int sessionId, string notes)
        {
            if (sessionId < 1 || string.IsNullOrWhiteSpace(notes))
            {
                return BadRequest("invalid data");
            }

            var session = await unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if (session == null)
            {
                return NotFound("Session not found.");
            }

            await unitOfWork.SessionRepository.UpdateDoctorNotesAsync(sessionId, notes);
            return Ok("Doctor notes added successfully.");
        }
    }
}
