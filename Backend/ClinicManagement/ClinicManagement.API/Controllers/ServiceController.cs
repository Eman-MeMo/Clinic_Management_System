using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.ServiceDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuditLoggerService auditLogger;
        private string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public ServiceController(IUnitOfWork _unitOfWork, IMapper _mapper, IAuditLoggerService _auditLogger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            auditLogger = _auditLogger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await unitOfWork.ServiceRepository.GetAllAsync();
            if (services == null || !services.Any())
            {
                return NotFound("No services found.");
            }
            var serviceDtos = mapper.Map<IEnumerable<ServiceDto>>(services);
            return Ok(serviceDtos);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var service = await unitOfWork.ServiceRepository.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound($"Service with ID {id} not found.");
            }
            var serviceDto = mapper.Map<ServiceDto>(service);
            return Ok(serviceDto);
        }
        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDto serviceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (serviceDto == null)
            {
                return BadRequest("Service data is null.");
            }

            var service = mapper.Map<Domain.Entities.Service>(serviceDto);
            await unitOfWork.ServiceRepository.AddAsync(service);
            await unitOfWork.SaveChangesAsync();

            // Log the creation of the service
            await auditLogger.LogAsync("Create", "Service", $"Created service with ID {service.Id}", UserId);

            var resultDto = mapper.Map<ServiceDto>(service);
            return CreatedAtAction(nameof(GetServiceById), new { id = service.Id }, resultDto);
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceDto serviceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (serviceDto == null || id != serviceDto.Id)
            {
                return BadRequest("Invalid service data!");
            }
            var existingService = await unitOfWork.ServiceRepository.GetByIdAsync(id);
            if (existingService == null)
            {
                return NotFound($"Service with ID {id} not found.");
            }
            mapper.Map(serviceDto, existingService);
            unitOfWork.ServiceRepository.Update(existingService);
            await unitOfWork.SaveChangesAsync();

            // Log the update of the service
            await auditLogger.LogAsync("Update", "Service", $"Updated service with ID {existingService.Id}", UserId);

            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var service = await unitOfWork.ServiceRepository.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound($"Service with ID {id} not found.");
            }
            unitOfWork.ServiceRepository.Delete(service);
            await unitOfWork.SaveChangesAsync();

            // Log the deletion of the service
            await auditLogger.LogAsync("Delete", "Service", $"Deleted service with ID {service.Id}", UserId);

            return NoContent();
        }
    }
}
