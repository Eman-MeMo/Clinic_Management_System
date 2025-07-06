using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.SpecializationDTOs;
using ClinicManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecializationController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuditLoggerService auditLogger;
        private string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public SpecializationController(IUnitOfWork _unitOfWork, IMapper _mapper,IAuditLoggerService _auditLogger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            auditLogger = _auditLogger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSpecializations()
        {
            var specializations = await unitOfWork.SpecializationRepository.GetAllAsync();
            if (specializations == null || !specializations.Any())
            {
                return NotFound("No specializations found.");
            }
            var specializationDtos = mapper.Map<IEnumerable<SpecializationDto>>(specializations);
            return Ok(specializationDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSpecializationById(int id)
        {
            var specialization = await unitOfWork.SpecializationRepository.GetByIdAsync(id);
            if (specialization == null)
            {
                return NotFound($"Specialization with ID {id} not found.");
            }
            var specializationDto = mapper.Map<SpecializationDto>(specialization);
            return Ok(specializationDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpecialization([FromBody] CreateSpecializationDto specializationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (specializationDto == null)
            {
                return BadRequest("Specialization data is null.");
            }
            var specialization = mapper.Map<Domain.Entities.Specialization>(specializationDto);
            await unitOfWork.SpecializationRepository.AddAsync(specialization);
            await unitOfWork.SaveChangesAsync();
            var resultDto = mapper.Map<SpecializationDto>(specialization);

            // Log the creation of the specialization
            await auditLogger.LogAsync("Create", "Specialization", $"Created specialization with ID {specialization.Id} and name {specialization.Name}", UserId);

            return CreatedAtAction(nameof(GetSpecializationById), new { id = specialization.Id }, resultDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSpecialization(int id, [FromBody] SpecializationDto specializationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (specializationDto == null || id != specializationDto.Id)
            {
                return BadRequest("Invalid specialization data.");
            }
            var existingSpecialization = await unitOfWork.SpecializationRepository.GetByIdAsync(id);
            if (existingSpecialization == null)
            {
                return NotFound($"Specialization with ID {id} not found.");
            }
            mapper.Map(specializationDto, existingSpecialization);
            unitOfWork.SpecializationRepository.Update(existingSpecialization);
            await unitOfWork.SaveChangesAsync();

            // Log the update of the specialization
            await auditLogger.LogAsync("Update", "Specialization", $"Updated specialization with ID {id} and name {existingSpecialization.Name}", UserId);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSpecialization(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var specialization = await unitOfWork.SpecializationRepository.GetByIdAsync(id);
            if (specialization == null)
            {
                return NotFound($"Specialization with ID {id} not found.");
            }
            unitOfWork.SpecializationRepository.Delete(specialization);
            await unitOfWork.SaveChangesAsync();

            // Log the deletion of the specialization
            await auditLogger.LogAsync("Delete", "Specialization", $"Deleted specialization with ID {id} and name {specialization.Name}", UserId);

            return NoContent();

        }
    }
}