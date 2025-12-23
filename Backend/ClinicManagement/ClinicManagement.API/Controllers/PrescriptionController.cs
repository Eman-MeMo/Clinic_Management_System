using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClinicManagement.Domain.DTOs.PrescriptionDTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Domain.DTOs.Pagination;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Patient,Doctor")]
    public class PrescriptionController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IPrescriptionService prescriptionService;

        public PrescriptionController(IUnitOfWork _unitOfWork, IMapper _mapper, IPrescriptionService _prescriptionService)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            prescriptionService = _prescriptionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPrescription([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var Prescriptions = unitOfWork.PrescriptionRepository.GetAllAsQueryable();
            var totalCount = await Prescriptions.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await Prescriptions.Skip(paginationSkip).Take(pageSize).ToListAsync();

            var prescriptionDtos = mapper.Map<List<PrescriptionDto>>(items);
            var result = new PaginatedResultDto<PrescriptionDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = prescriptionDtos
            };
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrescriptionById(int id)
        {
            if (id < 1)
            {
                return BadRequest("Prescription ID cannot be null or empty.");
            }

            var prescription = await unitOfWork.PrescriptionRepository.GetByIdAsync(id);
            if (prescription == null)
            {
                return NotFound("Prescription not found.");
            }
            var prescriptionDto = mapper.Map<PrescriptionDto>(prescription);
            return Ok(prescriptionDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionDto prescriptionDto)
        {
            if (prescriptionDto == null)
            {
                return BadRequest("Invalid Prescription ID.");
            }
            var prescriptionDtoFinal = await prescriptionService.CreatePrescriptionAsync(prescriptionDto);

            return CreatedAtAction(nameof(GetPrescriptionById), new { id = prescriptionDtoFinal.Id }, prescriptionDtoFinal);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(int id, [FromBody] PrescriptionDto prescriptionDto)
        {
            if (prescriptionDto == null || id != prescriptionDto.Id)
            {
                return BadRequest("Prescription data is invalid.");
            }

            var existingPrescription = await unitOfWork.PrescriptionRepository.GetByIdAsync(id);
            if (existingPrescription == null)
            {
                return NotFound($"Prescription with ID {id} not found.");
            }

            mapper.Map(prescriptionDto, existingPrescription);
            unitOfWork.PrescriptionRepository.Update(existingPrescription);
            await unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            var prescription = await unitOfWork.PrescriptionRepository.GetByIdAsync(id);
            if (prescription == null)
            {
                return NotFound($"Prescription with ID {id} not found.");
            }

            unitOfWork.PrescriptionRepository.Delete(prescription);
            await unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetPrescriptionsByPatientId(string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                return BadRequest("Patient ID is required.");
            }

            var prescriptions = await unitOfWork.PrescriptionRepository.GetByPatientIdAsync(patientId);
            if (prescriptions == null || !prescriptions.Any())
            {
                return NotFound($"No prescriptions found for patient with ID {patientId}.");
            }
            var prescriptionDtos = mapper.Map<IEnumerable<PrescriptionDto>>(prescriptions);
            return Ok(prescriptionDtos);
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetPrescriptionsBySessionId(int sessionId)
        {
            if (sessionId <= 0)
            {
                return BadRequest("Invalid session ID.");
            }

            var prescriptions = await unitOfWork.PrescriptionRepository.GetBySessionIdAsync(sessionId);
            if (prescriptions == null || !prescriptions.Any())
            {
                return NotFound($"No prescriptions found for session with ID {sessionId}.");
            }
            var prescriptionDtos = mapper.Map<IEnumerable<PrescriptionDto>>(prescriptions);
            return Ok(prescriptionDtos);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetPrescriptionsByDoctorId(string doctorId)
        {
            if (string.IsNullOrEmpty(doctorId))
            {
                return BadRequest("Doctor ID is required.");
            }

            var prescriptions = await unitOfWork.PrescriptionRepository.GetByDoctorIdAsync(doctorId);
            if (prescriptions == null || !prescriptions.Any())
            {
                return NotFound($"No prescriptions found for doctor with ID {doctorId}.");
            }
            var prescriptionDtos = mapper.Map<IEnumerable<PrescriptionDto>>(prescriptions);
            return Ok(prescriptionDtos);
        }
    }
}
