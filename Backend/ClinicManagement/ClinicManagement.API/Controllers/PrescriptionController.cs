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

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<PrescriptionController> logger;

        public PrescriptionController(IUnitOfWork _unitOfWork, IMapper _mapper, ILogger<PrescriptionController> _logger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            logger = _logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPrescription([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("Fetching all prescriptions with pagination. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

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

            logger.LogInformation("Retrieved {Count} prescriptions.", items.Count);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrescriptionById(int id)
        {
            logger.LogInformation("Fetching prescription by ID: {Id}", id);

            if (id < 1)
            {
                logger.LogWarning("Invalid prescription ID: {Id}", id);
                return BadRequest("Prescription ID cannot be null or empty.");
            }

            var prescription = await unitOfWork.PrescriptionRepository.GetByIdAsync(id);
            if (prescription == null)
            {
                logger.LogWarning("Prescription with ID {Id} not found.", id);
                return NotFound("Prescription not found.");
            }

            logger.LogInformation("Prescription with ID {Id} retrieved.", id);
            var prescriptionDto = mapper.Map<PrescriptionDto>(prescription);
            return Ok(prescriptionDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionDto prescriptionDto)
        {
            logger.LogInformation("Creating a new prescription.");

            if (prescriptionDto == null)
            {
                logger.LogWarning("Received null prescription DTO.");
                return BadRequest("Invalid Prescription ID.");
            }

            var prescription = mapper.Map<Prescription>(prescriptionDto);
            await unitOfWork.PrescriptionRepository.AddAsync(prescription);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Prescription created with ID: {Id}", prescription.Id);

            await unitOfWork.PrescriptionRepository.CreateMedicalRecoredAsync(prescriptionDto.Notes, prescriptionDto.Diagnosis, prescription.Id);

            logger.LogInformation("Associated medical record created for prescription ID: {Id}", prescription.Id);

            var prescriptionDtoFinal = mapper.Map<PrescriptionDto>(prescription);
            return CreatedAtAction(nameof(GetPrescriptionById), new { id = prescription.Id }, prescriptionDtoFinal);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(int id, [FromBody] PrescriptionDto prescriptionDto)
        {
            logger.LogInformation("Updating prescription with ID: {Id}", id);

            if (prescriptionDto == null || id != prescriptionDto.Id)
            {
                logger.LogWarning("Prescription data invalid or ID mismatch.");
                return BadRequest("Prescription data is invalid.");
            }

            var existingPrescription = await unitOfWork.PrescriptionRepository.GetByIdAsync(id);
            if (existingPrescription == null)
            {
                logger.LogWarning("Prescription with ID {Id} not found.", id);
                return NotFound($"Prescription with ID {id} not found.");
            }

            mapper.Map(prescriptionDto, existingPrescription);
            unitOfWork.PrescriptionRepository.Update(existingPrescription);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Prescription with ID {Id} updated successfully.", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            logger.LogInformation("Deleting prescription with ID: {Id}", id);

            var prescription = await unitOfWork.PrescriptionRepository.GetByIdAsync(id);
            if (prescription == null)
            {
                logger.LogWarning("Prescription with ID {Id} not found for deletion.", id);
                return NotFound($"Prescription with ID {id} not found.");
            }

            unitOfWork.PrescriptionRepository.Delete(prescription);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Prescription with ID {Id} deleted.", id);
            return NoContent();
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetPrescriptionsByPatientId(string patientId)
        {
            logger.LogInformation("Fetching prescriptions by patient ID: {PatientId}", patientId);

            if (string.IsNullOrEmpty(patientId))
            {
                logger.LogWarning("Patient ID is null or empty.");
                return BadRequest("Patient ID is required.");
            }

            var prescriptions = await unitOfWork.PrescriptionRepository.GetByPatientIdAsync(patientId);
            if (prescriptions == null || !prescriptions.Any())
            {
                logger.LogWarning("No prescriptions found for patient ID: {PatientId}", patientId);
                return NotFound($"No prescriptions found for patient with ID {patientId}.");
            }

            logger.LogInformation("Retrieved {Count} prescriptions for patient ID {PatientId}.", prescriptions.Count(), patientId);
            var prescriptionDtos = mapper.Map<IEnumerable<PrescriptionDto>>(prescriptions);
            return Ok(prescriptionDtos);
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetPrescriptionsBySessionId(int sessionId)
        {
            logger.LogInformation("Fetching prescriptions by session ID: {SessionId}", sessionId);

            if (sessionId <= 0)
            {
                logger.LogWarning("Invalid session ID: {SessionId}", sessionId);
                return BadRequest("Invalid session ID.");
            }

            var prescriptions = await unitOfWork.PrescriptionRepository.GetBySessionIdAsync(sessionId);
            if (prescriptions == null || !prescriptions.Any())
            {
                logger.LogWarning("No prescriptions found for session ID: {SessionId}", sessionId);
                return NotFound($"No prescriptions found for session with ID {sessionId}.");
            }

            logger.LogInformation("Retrieved {Count} prescriptions for session ID {SessionId}.", prescriptions.Count(), sessionId);
            var prescriptionDtos = mapper.Map<IEnumerable<PrescriptionDto>>(prescriptions);
            return Ok(prescriptionDtos);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetPrescriptionsByDoctorId(string doctorId)
        {
            logger.LogInformation("Fetching prescriptions by doctor ID: {DoctorId}", doctorId);

            if (string.IsNullOrEmpty(doctorId))
            {
                logger.LogWarning("Doctor ID is null or empty.");
                return BadRequest("Doctor ID is required.");
            }

            var prescriptions = await unitOfWork.PrescriptionRepository.GetByDoctorIdAsync(doctorId);
            if (prescriptions == null || !prescriptions.Any())
            {
                logger.LogWarning("No prescriptions found for doctor ID: {DoctorId}", doctorId);
                return NotFound($"No prescriptions found for doctor with ID {doctorId}.");
            }

            logger.LogInformation("Retrieved {Count} prescriptions for doctor ID {DoctorId}.", prescriptions.Count(), doctorId);
            var prescriptionDtos = mapper.Map<IEnumerable<PrescriptionDto>>(prescriptions);
            return Ok(prescriptionDtos);
        }
    }
}
