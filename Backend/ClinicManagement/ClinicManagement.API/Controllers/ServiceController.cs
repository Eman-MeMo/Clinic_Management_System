using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.ServiceDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<ServiceController> logger;

        public ServiceController(IUnitOfWork _unitOfWork, IMapper _mapper, ILogger<ServiceController> _logger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            logger = _logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            logger.LogInformation("Fetching all services.");
            var services = await unitOfWork.ServiceRepository.GetAllAsync();
            if (services == null || !services.Any())
            {
                logger.LogWarning("No services found.");
                return NotFound("No services found.");
            }
            var serviceDtos = mapper.Map<IEnumerable<ServiceDto>>(services);
            logger.LogInformation("Retrieved {Count} services.", serviceDtos.Count());
            return Ok(serviceDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            logger.LogInformation("Fetching service with ID {Id}.", id);
            var service = await unitOfWork.ServiceRepository.GetByIdAsync(id);
            if (service == null)
            {
                logger.LogWarning("Service with ID {Id} not found.", id);
                return NotFound($"Service with ID {id} not found.");
            }
            var serviceDto = mapper.Map<ServiceDto>(service);
            logger.LogInformation("Service with ID {Id} retrieved successfully.", id);
            return Ok(serviceDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDto serviceDto)
        {
            if (serviceDto == null)
            {
                logger.LogWarning("Received null service data in create request.");
                return BadRequest("Service data is null.");
            }

            var service = mapper.Map<Domain.Entities.Service>(serviceDto);
            await unitOfWork.ServiceRepository.AddAsync(service);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Created new service with ID {Id}.", service.Id);

            var resultDto = mapper.Map<ServiceDto>(service);
            return CreatedAtAction(nameof(GetServiceById), new { id = service.Id }, resultDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceDto serviceDto)
        {
            if (serviceDto == null || id != serviceDto.Id)
            {
                logger.LogWarning("Invalid service data for update. Provided ID: {Id}, DTO ID: {DtoId}.", id, serviceDto?.Id);
                return BadRequest("Invalid service data!");
            }

            var existingService = await unitOfWork.ServiceRepository.GetByIdAsync(id);
            if (existingService == null)
            {
                logger.LogWarning("Service with ID {Id} not found for update.", id);
                return NotFound($"Service with ID {id} not found.");
            }

            mapper.Map(serviceDto, existingService);
            unitOfWork.ServiceRepository.Update(existingService);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Service with ID {Id} updated successfully.", id);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            logger.LogInformation("Attempting to delete service with ID {Id}.", id);
            var service = await unitOfWork.ServiceRepository.GetByIdAsync(id);
            if (service == null)
            {
                logger.LogWarning("Service with ID {Id} not found for deletion.", id);
                return NotFound($"Service with ID {id} not found.");
            }

            unitOfWork.ServiceRepository.Delete(service);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Service with ID {Id} deleted successfully.", id);
            return NoContent();
        }
    }
}
