using AutoMapper;
using ClinicManagement.Application.Commands.Prescriptions.CreateMedicalRecord;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.MedicalRecords.GetLatestMedicalRecordByPatientId;
using ClinicManagement.Application.Queries.MedicalRecords.GetMedicalRecordsByDate;
using ClinicManagement.Application.Queries.MedicalRecords.GetMedicalRecordsByPatientId;
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
        private readonly IMediator mediator;
        public MedicalRecordController(IUnitOfWork _unitOfWork, IMapper _mapper,IMediator _mediator)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            mediator = _mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMedicalRecords([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
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
            return NoContent();
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetMedicalRecordsByPatientId(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                return BadRequest("Patient ID cannot be null or empty.");
            }

            var records = await mediator.Send(new GetMedicalRecordsByPatientIdQuery { PatientId = patientId });
            return Ok(records);
        }

        [HttpGet("patient/{patientId}/latest")]
        public async Task<IActionResult> GetLatestMedicalRecordByPatientId(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                return BadRequest("Patient ID cannot be null or empty.");
            }

            var record = await mediator.Send(new GetLatestMedicalRecordByPatientIdQuery { PatientId = patientId });
            return Ok(record);
        }

        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetMedicalRecordsByDate(DateTime date)
        {
            if (date > DateTime.Now)
            {
                return BadRequest("Date cannot be in the future.");
            }

            var records = await mediator.Send(new GetMedicalRecordsByDateQuery { date = date });
            return Ok(records);
        }
    }
}
