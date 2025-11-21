using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using ClinicManagement.Domain.DTOs.SessionServiceDTOs;
using ClinicManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SessionServiceController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public SessionServiceController(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }

        [HttpGet("{sessionId}")]
        public async Task<IActionResult> GetSessionServices(int sessionId)
        {
            if (sessionId == 0)
            {
                return BadRequest("Invalid session ID.");
            }

            var services = await unitOfWork.SessionServiceRepository.GetAllBySessionIdAsync(sessionId);

            if (services == null || !services.Any())
            {
                return NotFound("No services found for this session.");
            }

            var serviceDtos = mapper.Map<IEnumerable<SessionServiceDto>>(services);
            return Ok(serviceDtos);
        }

        [HttpGet("{sessionId}/{serviceId}")]
        public async Task<IActionResult> GetSessionServiceById(int sessionId, int serviceId)
        {
            if (sessionId == 0 || serviceId == 0)
            {
                return BadRequest("Invalid session or service ID.");
            }

            var sessionService = await unitOfWork.SessionServiceRepository.GetByIdAsync(sessionId, serviceId);

            if (sessionService == null)
            {
                return NotFound($"SessionService with Session ID {sessionId} and Service ID {serviceId} not found.");
            }

            var sessionServiceDto = mapper.Map<SessionServiceDto>(sessionService);
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

            var resultDto = mapper.Map<SessionServiceDto>(sessionService);
            return CreatedAtAction(nameof(GetSessionServiceById), new { sessionId = sessionService.SessionId, serviceId = sessionService.ServiceId }, resultDto);
        }

        [HttpDelete("{sessionId}/{serviceId}")]
        public async Task<IActionResult> DeleteSessionService(int sessionId, int serviceId)
        {
            var sessionService = await unitOfWork.SessionServiceRepository.GetByIdAsync(sessionId, serviceId);

            if (sessionService == null)
            {
                return NotFound("SessionService not found.");
            }

            unitOfWork.SessionServiceRepository.Delete(sessionService);
            await unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
}
