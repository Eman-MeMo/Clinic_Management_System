using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.MedicalRecordDTOs;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuditLoggerService auditLogger;
        private string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public MedicalRecordController(IUnitOfWork _unitOfWork, IMapper _mapper, IAuditLoggerService _auditLogger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            auditLogger = _auditLogger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMedicalRecords([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var records =  unitOfWork.MedicalRecordRepository.GetAllAsQueryable();
            var totalCount = await records.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await records
                .Skip(paginationSkip)
                .Take(pageSize)
                .ToListAsync();

            var medicalRecordDtos = mapper.Map<List<MedicalRecordDto>>(items);
            var result = new PaginatedResultDto<MedicalRecordDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = medicalRecordDtos
            };
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicalRecordById(int id)
        {
            var record = await unitOfWork.MedicalRecordRepository.GetByIdAsync(id);
            if (record == null)
            {
                return NotFound("Medical record not found.");
            }
            var mappedRecord = mapper.Map<MedicalRecordDto>(record);
            return Ok(mappedRecord);
        }
        [HttpPost]
        public async Task<IActionResult> CreateMedicalRecord([FromBody] CreateMedicalRecordDto medicalRecordDto)
        {
            if (medicalRecordDto == null)
            {
                return BadRequest("Medical record data cannot be null.");
            }
            var newRecord = mapper.Map<MedicalRecord>(medicalRecordDto);
            await unitOfWork.MedicalRecordRepository.AddAsync(newRecord);
            await unitOfWork.SaveChangesAsync();

            // Log the creation of the medical record
            await auditLogger.LogAsync("Create", "MedicalRecords", $"Created record for patient {newRecord.PatientId}", UserId);
            return CreatedAtAction(nameof(GetMedicalRecordById), new { id = newRecord.Id }, medicalRecordDto);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicalRecord(int id, [FromBody] MedicalRecordDto medicalRecordDto)
        {
            if (id != medicalRecordDto.Id)
            {
                return BadRequest("Medical record ID mismatch.");
            }
            var existingRecord = await unitOfWork.MedicalRecordRepository.GetByIdAsync(id);
            if (existingRecord == null)
            {
                return NotFound("Medical record not found.");
            }
            var updatedRecord = mapper.Map(medicalRecordDto, existingRecord);
            unitOfWork.MedicalRecordRepository.Update(updatedRecord);
            await unitOfWork.SaveChangesAsync();

            // Log the update of the medical record
            await auditLogger.LogAsync("Update", "MedicalRecords", $"Updated record for patient {updatedRecord.PatientId}", UserId);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalRecord(int id)
        {
            var record = await unitOfWork.MedicalRecordRepository.GetByIdAsync(id);
            if (record == null)
            {
                return NotFound("Medical record not found.");
            }
            unitOfWork.MedicalRecordRepository.Delete(record);
            await unitOfWork.SaveChangesAsync();

            // Log the deletion of the medical record
            await auditLogger.LogAsync("Delete", "MedicalRecords", $"Deleted record for patient {record.PatientId}", UserId);
            return NoContent();
        }
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetMedicalRecordsByPatientId(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                return BadRequest("Patient ID cannot be null or empty.");
            }
            var records = await unitOfWork.MedicalRecordRepository.GetByPatientIdAsync(patientId);
            if (records == null || !records.Any())
            {
                return NotFound("No medical records found for this patient.");
            }
            var mappedRecords = mapper.Map<IEnumerable<MedicalRecordDto>>(records);
            return Ok(mappedRecords);
        }
        [HttpGet("patient/{patientId}/latest")]
        public async Task<IActionResult> GetLatestMedicalRecordByPatientId(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                return BadRequest("Patient ID cannot be null or empty.");
            }
            var record = await unitOfWork.MedicalRecordRepository.GetLatestRecordAsync(patientId);
            if (record == null)
            {
                return NotFound("No medical records found for this patient.");
            }
            var mappedRecord = mapper.Map<MedicalRecordDto>(record);
            return Ok(mappedRecord);
        }
        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetMedicalRecordsByDate(DateTime date)
        {
            if (date > DateTime.Now)
            {
                return BadRequest("Date cannot be in the future.");
            }
            var records = await unitOfWork.MedicalRecordRepository.GetByDateAsync(date);
            if (records == null || !records.Any())
            {
                return NotFound("No medical records found for this date.");
            }
            var mappedRecords = mapper.Map<IEnumerable<MedicalRecordDto>>(records);
            return Ok(mappedRecords);
        }
    }
}
