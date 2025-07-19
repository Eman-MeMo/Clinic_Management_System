using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using ClinicManagement.Domain.DTOs.SessionServiceDTOs;
using ClinicManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionServiceController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuditLoggerService auditLogger;
        private string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public SessionServiceController(IUnitOfWork _unitOfWork, IMapper _mapper, IAuditLoggerService _auditLogger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            auditLogger = _auditLogger;
        }

        [HttpGet("{sessionId}")]
        public async Task<IActionResult> GetSessionServices(int sessionId)
        {
            if (sessionId == 0)
                return BadRequest("Invalid session ID.");

            var services = await unitOfWork.SessionServiceRepository.GetAllBySessionIdAsync(sessionId);

            if (services == null || !services.Any())
                return NotFound("No services found for this session.");

            var serviceDtos = mapper.Map<IEnumerable<SessionServiceDto>>(services);

            // Log the action
            await auditLogger.LogAsync("Get Session Services", "SessionService", $"Retrieved services for session ID {sessionId}", UserId);

            return Ok(serviceDtos);
        }
        [HttpGet("service/{serviceId}/session/{sessionId}")]
        public async Task<IActionResult> GetSessionServiceById(int sessionId, int serviceId)
        {
            if (sessionId == 0 || serviceId == 0)
                return BadRequest("Invalid session or service ID.");
            var sessionService = await unitOfWork.SessionServiceRepository.GetByIdAsync(sessionId, serviceId);
            if (sessionService == null)
            {
                return NotFound($"SessionService with Session ID {sessionId} and Service ID {serviceId} not found.");
            }
            var sessionServiceDto = mapper.Map<SessionServiceDto>(sessionService);
            // Log the action
            await auditLogger.LogAsync("Get Session Service", "SessionService", $"Retrieved session service with Session ID {sessionId} and Service ID {serviceId}", UserId);
            return Ok(sessionServiceDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSessionService(SessionServiceDto sessionServiceDto)
        {
            if (sessionServiceDto == null)
            {
                return BadRequest("SessionService data is null.");
            }
            var exists = await unitOfWork.SessionServiceRepository.ExistsAsync(sessionServiceDto.SessionId, sessionServiceDto.ServiceId);
            if (exists)
            {
                return Conflict("This service is already linked to the session.");
            }

            var sessionService = mapper.Map<Domain.Entities.SessionService>(sessionServiceDto);
            await unitOfWork.SessionServiceRepository.AddAsync(sessionService);
            await unitOfWork.SaveChangesAsync();

            // Log the action
            await auditLogger.LogAsync("Create Session Service", "SessionService", $"Created a new session service with Session ID {sessionService.SessionId} and Service ID {sessionService.ServiceId}", UserId);

            return CreatedAtAction(nameof(GetSessionServices), new { sessionId = sessionService.SessionId }, sessionService);
        }
        [HttpDelete("{sessionId}/{serviceId}")]
        public async Task<IActionResult> DeleteSessionService(int sessionId,int serviceId)
        {
            var sessionService = await unitOfWork.SessionServiceRepository.GetByIdAsync(sessionId,serviceId);
            if (sessionService == null)
            {
                return NotFound($"SessionService not found.");
            }
            unitOfWork.SessionServiceRepository.Delete(sessionService);
            await unitOfWork.SaveChangesAsync();

            // Log the action
            await auditLogger.LogAsync("Delete Session Service", "SessionService", $"Deleted session service with Session ID {sessionId} and Service ID {serviceId}", UserId);

            return NoContent();
        }
    }
}
