using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using ClinicManagement.Domain.DTOs.SessionServiceDTOs;
using ClinicManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionServiceController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<SessionServiceController> logger;

        public SessionServiceController(IUnitOfWork _unitOfWork, IMapper _mapper, ILogger<SessionServiceController> _logger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            logger = _logger;
        }

        [HttpGet("{sessionId}")]
        public async Task<IActionResult> GetSessionServices(int sessionId)
        {
            logger.LogInformation("Fetching services for session ID: {SessionId}", sessionId);

            if (sessionId == 0)
            {
                logger.LogWarning("Invalid session ID: 0");
                return BadRequest("Invalid session ID.");
            }

            var services = await unitOfWork.SessionServiceRepository.GetAllBySessionIdAsync(sessionId);

            if (services == null || !services.Any())
            {
                logger.LogWarning("No services found for session ID: {SessionId}", sessionId);
                return NotFound("No services found for this session.");
            }

            var serviceDtos = mapper.Map<IEnumerable<SessionServiceDto>>(services);

            logger.LogInformation("Successfully retrieved {Count} services for session ID: {SessionId}", serviceDtos.Count(), sessionId);
            return Ok(serviceDtos);
        }

        [HttpGet("{sessionId}/{serviceId}")]
        public async Task<IActionResult> GetSessionServiceById(int sessionId, int serviceId)
        {
            logger.LogInformation("Fetching session service with Session ID: {SessionId}, Service ID: {ServiceId}", sessionId, serviceId);

            if (sessionId == 0 || serviceId == 0)
            {
                logger.LogWarning("Invalid session ID or service ID provided.");
                return BadRequest("Invalid session or service ID.");
            }

            var sessionService = await unitOfWork.SessionServiceRepository.GetByIdAsync(sessionId, serviceId);

            if (sessionService == null)
            {
                logger.LogWarning("SessionService not found with Session ID: {SessionId} and Service ID: {ServiceId}", sessionId, serviceId);
                return NotFound($"SessionService with Session ID {sessionId} and Service ID {serviceId} not found.");
            }

            var sessionServiceDto = mapper.Map<SessionServiceDto>(sessionService);

            logger.LogInformation("Successfully retrieved SessionService: Session ID {SessionId}, Service ID {ServiceId}", sessionId, serviceId);
            return Ok(sessionServiceDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSessionService(SessionServiceDto sessionServiceDto)
        {
            logger.LogInformation("Creating new session service link: Session ID {SessionId}, Service ID {ServiceId}", sessionServiceDto?.SessionId, sessionServiceDto?.ServiceId);

            if (sessionServiceDto == null)
            {
                logger.LogWarning("SessionServiceDto is null.");
                return BadRequest("SessionService data is null.");
            }

            var exists = await unitOfWork.SessionServiceRepository.ExistsAsync(sessionServiceDto.SessionId, sessionServiceDto.ServiceId);

            if (exists)
            {
                logger.LogWarning("SessionService already exists for Session ID: {SessionId}, Service ID: {ServiceId}", sessionServiceDto.SessionId, sessionServiceDto.ServiceId);
                return Conflict("This service is already linked to the session.");
            }

            var sessionService = mapper.Map<Domain.Entities.SessionService>(sessionServiceDto);
            await unitOfWork.SessionServiceRepository.AddAsync(sessionService);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Successfully created SessionService for Session ID: {SessionId}, Service ID: {ServiceId}", sessionService.SessionId, sessionService.ServiceId);

            var resultDto = mapper.Map<SessionServiceDto>(sessionService);
            return CreatedAtAction(nameof(GetSessionServiceById), new { sessionId = sessionService.SessionId, serviceId = sessionService.ServiceId }, resultDto);
        }

        [HttpDelete("{sessionId}/{serviceId}")]
        public async Task<IActionResult> DeleteSessionService(int sessionId, int serviceId)
        {
            logger.LogInformation("Attempting to delete SessionService with Session ID: {SessionId}, Service ID: {ServiceId}", sessionId, serviceId);

            var sessionService = await unitOfWork.SessionServiceRepository.GetByIdAsync(sessionId, serviceId);

            if (sessionService == null)
            {
                logger.LogWarning("SessionService not found for deletion. Session ID: {SessionId}, Service ID: {ServiceId}", sessionId, serviceId);
                return NotFound("SessionService not found.");
            }

            unitOfWork.SessionServiceRepository.Delete(sessionService);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Successfully deleted SessionService with Session ID: {SessionId}, Service ID: {ServiceId}", sessionId, serviceId);

            return NoContent();
        }
    }
}
