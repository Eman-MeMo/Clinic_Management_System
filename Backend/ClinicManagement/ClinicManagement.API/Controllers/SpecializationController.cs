using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.SpecializationDTOs;
using ClinicManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecializationController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<SpecializationController> logger;

        public SpecializationController(IUnitOfWork _unitOfWork, IMapper _mapper, ILogger<SpecializationController> _logger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            logger = _logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSpecializations()
        {
            logger.LogInformation("Fetching all specializations.");
            var specializations = await unitOfWork.SpecializationRepository.GetAllAsync();
            if (specializations == null || !specializations.Any())
            {
                logger.LogWarning("No specializations found.");
                return NotFound("No specializations found.");
            }
            var specializationDtos = mapper.Map<IEnumerable<SpecializationDto>>(specializations);
            logger.LogInformation("Successfully retrieved {Count} specializations.", specializationDtos.Count());
            return Ok(specializationDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSpecializationById(int id)
        {
            logger.LogInformation("Fetching specialization with ID: {Id}", id);
            var specialization = await unitOfWork.SpecializationRepository.GetByIdAsync(id);
            if (specialization == null)
            {
                logger.LogWarning("Specialization with ID {Id} not found.", id);
                return NotFound($"Specialization with ID {id} not found.");
            }
            var specializationDto = mapper.Map<SpecializationDto>(specialization);
            logger.LogInformation("Successfully retrieved specialization with ID: {Id}", id);
            return Ok(specializationDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpecialization([FromBody] CreateSpecializationDto specializationDto)
        {
            if (specializationDto == null)
            {
                logger.LogWarning("Attempted to create specialization with null data.");
                return BadRequest("Specialization data is null.");
            }
            var specialization = mapper.Map<Domain.Entities.Specialization>(specializationDto);
            await unitOfWork.SpecializationRepository.AddAsync(specialization);
            await unitOfWork.SaveChangesAsync();
            var resultDto = mapper.Map<SpecializationDto>(specialization);

            logger.LogInformation("Specialization created with ID: {Id}", specialization.Id);

            return CreatedAtAction(nameof(GetSpecializationById), new { id = specialization.Id }, resultDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSpecialization(int id, [FromBody] SpecializationDto specializationDto)
        {
            if (specializationDto == null || id != specializationDto.Id)
            {
                logger.LogWarning("Invalid specialization data provided for update. ID: {Id}", id);
                return BadRequest("Invalid specialization data.");
            }

            var existingSpecialization = await unitOfWork.SpecializationRepository.GetByIdAsync(id);
            if (existingSpecialization == null)
            {
                logger.LogWarning("Specialization with ID {Id} not found for update.", id);
                return NotFound($"Specialization with ID {id} not found.");
            }

            mapper.Map(specializationDto, existingSpecialization);
            unitOfWork.SpecializationRepository.Update(existingSpecialization);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Specialization with ID {Id} updated successfully.", id);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSpecialization(int id)
        {
            logger.LogInformation("Attempting to delete specialization with ID: {Id}", id);
            var specialization = await unitOfWork.SpecializationRepository.GetByIdAsync(id);
            if (specialization == null)
            {
                logger.LogWarning("Specialization with ID {Id} not found for deletion.", id);
                return NotFound($"Specialization with ID {id} not found.");
            }

            unitOfWork.SpecializationRepository.Delete(specialization);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Specialization with ID {Id} deleted successfully.", id);

            return NoContent();
        }
    }
}
