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
        public SpecializationController(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
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
            if (specializationDto == null)
            {
                return BadRequest("Specialization data is null.");
            }
            var specialization = mapper.Map<Domain.Entities.Specialization>(specializationDto);
            await unitOfWork.SpecializationRepository.AddAsync(specialization);
            await unitOfWork.SaveChangesAsync();
            var resultDto = mapper.Map<SpecializationDto>(specialization);
            return CreatedAtAction(nameof(GetSpecializationById), new { id = specialization.Id }, resultDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSpecialization(int id, [FromBody] SpecializationDto specializationDto)
        {
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
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSpecialization(int id)
        {
            var specialization = await unitOfWork.SpecializationRepository.GetByIdAsync(id);
            if (specialization == null)
            {
                return NotFound($"Specialization with ID {id} not found.");
            }

            unitOfWork.SpecializationRepository.Delete(specialization);
            await unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
}
