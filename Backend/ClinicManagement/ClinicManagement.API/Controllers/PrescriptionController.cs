using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClinicManagement.Domain.DTOs.PrescriptionDTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Domain.DTOs.Pagination;
namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuditLoggerService auditLogger;
        private string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public PrescriptionController(IUnitOfWork _unitOfWork, IMapper _mapper, IAuditLoggerService _auditLogger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            auditLogger = _auditLogger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPrescription([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var Prescriptions =  unitOfWork.PrescriptionRepository.GetAllAsQueryable();

            var totalCount = await Prescriptions.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await Prescriptions
                .Skip(paginationSkip)
                .Take(pageSize)
                .ToListAsync();

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
            var prescription = mapper.Map<Prescription>(prescriptionDto);
            await unitOfWork.PrescriptionRepository.AddAsync(prescription);
            await unitOfWork.SaveChangesAsync();

            // Create Medical Record Automatically
            await unitOfWork.PrescriptionRepository.CreateMedicalRecoredAsync(prescriptionDto.Notes, prescriptionDto.Diagnosis, prescription.Id);

            // Log the creation of the prescription
            await auditLogger.LogAsync("Create", "Prescriptions", $"Created Prescription with ID {prescription.Id}", UserId);

            var prescriptionDtoFinal = mapper.Map<PrescriptionDto>(prescription);
            return CreatedAtAction(nameof(GetPrescriptionById), new { id = prescription.Id }, prescriptionDtoFinal);
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

            // Log the update of the prescription
            await auditLogger.LogAsync("Update", "Prescriptions", $"Updated Prescription with ID {existingPrescription.Id}", UserId);
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

            // Log the deletion of the prescription
            await auditLogger.LogAsync("Delete", "Prescriptions", $"Deleted Prescription with ID {prescription.Id}", UserId);
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
