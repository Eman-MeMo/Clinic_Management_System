using AutoMapper;
using ClinicManagement.Application.Commands.Prescriptions.CreateMedicalRecord;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.MedicalRecordDTOs;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<MedicalRecordController> logger;

        public MedicalRecordController(IUnitOfWork _unitOfWork, IMapper _mapper, ILogger<MedicalRecordController> _logger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            logger = _logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMedicalRecords([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("Fetching all medical records with pagination: Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);
            var records = unitOfWork.MedicalRecordRepository.GetAllAsQueryable();
            var totalCount = await records.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await records.Skip(paginationSkip).Take(pageSize).ToListAsync();

            var medicalRecordDtos = mapper.Map<List<MedicalRecordDto>>(items);
            var result = new PaginatedResultDto<MedicalRecordDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = medicalRecordDtos
            };
            logger.LogInformation("Returned {Count} medical records", items.Count);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicalRecordById(int id)
        {
            logger.LogInformation("Fetching medical record with ID {Id}", id);
            var record = await unitOfWork.MedicalRecordRepository.GetByIdAsync(id);
            if (record == null)
            {
                logger.LogWarning("Medical record with ID {Id} not found", id);
                return NotFound("Medical record not found.");
            }
            var mappedRecord = mapper.Map<MedicalRecordDto>(record);
            logger.LogInformation("Medical record with ID {Id} retrieved successfully", id);
            return Ok(mappedRecord);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicalRecord(int id, [FromBody] MedicalRecordDto medicalRecordDto)
        {
            if (id != medicalRecordDto.Id)
            {
                logger.LogWarning("Medical record ID mismatch: Route ID {RouteId}, Body ID {BodyId}", id, medicalRecordDto.Id);
                return BadRequest("Medical record ID mismatch.");
            }

            var existingRecord = await unitOfWork.MedicalRecordRepository.GetByIdAsync(id);
            if (existingRecord == null)
            {
                logger.LogWarning("Medical record with ID {Id} not found for update", id);
                return NotFound("Medical record not found.");
            }

            var updatedRecord = mapper.Map(medicalRecordDto, existingRecord);
            unitOfWork.MedicalRecordRepository.Update(updatedRecord);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Updated medical record with ID {Id}", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalRecord(int id)
        {
            logger.LogInformation("Attempting to delete medical record with ID {Id}", id);
            var record = await unitOfWork.MedicalRecordRepository.GetByIdAsync(id);
            if (record == null)
            {
                logger.LogWarning("Medical record with ID {Id} not found for deletion", id);
                return NotFound("Medical record not found.");
            }

            unitOfWork.MedicalRecordRepository.Delete(record);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Deleted medical record with ID {Id}", id);
            return NoContent();
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetMedicalRecordsByPatientId(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                logger.LogWarning("Patient ID is null or empty in GetMedicalRecordsByPatientId");
                return BadRequest("Patient ID cannot be null or empty.");
            }

            var records = await unitOfWork.MedicalRecordRepository.GetByPatientIdAsync(patientId);
            if (records == null || !records.Any())
            {
                logger.LogInformation("No medical records found for patient ID {PatientId}", patientId);
                return NotFound("No medical records found for this patient.");
            }

            logger.LogInformation("Fetched {Count} medical records for patient ID {PatientId}", records.Count(), patientId);
            var mappedRecords = mapper.Map<IEnumerable<MedicalRecordDto>>(records);
            return Ok(mappedRecords);
        }

        [HttpGet("patient/{patientId}/latest")]
        public async Task<IActionResult> GetLatestMedicalRecordByPatientId(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                logger.LogWarning("Patient ID is null or empty in GetLatestMedicalRecordByPatientId");
                return BadRequest("Patient ID cannot be null or empty.");
            }

            var record = await unitOfWork.MedicalRecordRepository.GetLatestRecordAsync(patientId);
            if (record == null)
            {
                logger.LogInformation("No latest medical record found for patient ID {PatientId}", patientId);
                return NotFound("No medical records found for this patient.");
            }

            logger.LogInformation("Fetched latest medical record with ID {Id} for patient ID {PatientId}", record.Id, patientId);
            var mappedRecord = mapper.Map<MedicalRecordDto>(record);
            return Ok(mappedRecord);
        }

        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetMedicalRecordsByDate(DateTime date)
        {
            if (date > DateTime.Now)
            {
                logger.LogWarning("Attempted to fetch medical records for future date {Date}", date);
                return BadRequest("Date cannot be in the future.");
            }

            var records = await unitOfWork.MedicalRecordRepository.GetByDateAsync(date);
            if (records == null || !records.Any())
            {
                logger.LogInformation("No medical records found on date {Date}", date);
                return NotFound("No medical records found for this date.");
            }

            logger.LogInformation("Fetched {Count} medical records on date {Date}", records.Count(), date);
            var mappedRecords = mapper.Map<IEnumerable<MedicalRecordDto>>(records);
            return Ok(mappedRecords);
        }
    }
}
