using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.DTOs.PatientDTOs;
using ClinicManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<AttendanceController> logger;

        public AttendanceController(IUnitOfWork _unitOfWork, IMapper _mapper, ILogger<AttendanceController> _logger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            logger = _logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAttendance([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("Fetching all attendance records. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            var attendances = unitOfWork.AttendanceRepository.GetAllAsQueryable();
            var totalCount = await attendances.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await attendances.Skip(paginationSkip).Take(pageSize).ToListAsync();

            var attendanceDtos = mapper.Map<List<AttendanceDto>>(items);
            var result = new PaginatedResultDto<AttendanceDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = attendanceDtos
            };
            logger.LogInformation("Returned {Count} attendance records.", attendanceDtos.Count);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttendanceById(int id)
        {
            logger.LogInformation("Getting attendance by ID: {Id}", id);
            if (id < 1)
            {
                logger.LogWarning("Invalid ID provided: {Id}", id);
                return BadRequest("Attendance ID cannot be null or empty.");
            }

            var attendance = await unitOfWork.AttendanceRepository.GetByIdAsync(id);
            if (attendance == null)
            {
                logger.LogWarning("Attendance not found for ID: {Id}", id);
                return NotFound("Attendance not found.");
            }

            var attendanceDto = mapper.Map<AttendanceDto>(attendance);
            return Ok(attendanceDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAttendance([FromBody] CreateAppointmentDto attendanceDto)
        {
            logger.LogInformation("Creating new attendance record.");
            var attendance = mapper.Map<Attendance>(attendanceDto);
            await unitOfWork.AttendanceRepository.AddAsync(attendance);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Created attendance with ID: {Id}", attendance.Id);

            return CreatedAtAction(nameof(GetAttendanceById), new { id = attendance.Id }, mapper.Map<AttendanceDto>(attendance));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttendance(int id, [FromBody] AttendanceDto attendanceDto)
        {
            logger.LogInformation("Updating attendance ID: {Id}", id);
            if (id != attendanceDto.Id)
            {
                logger.LogWarning("Mismatched ID in update request: {RequestId} != {DtoId}", id, attendanceDto.Id);
                return BadRequest("Invalid data");
            }

            var existingAttendance = await unitOfWork.AttendanceRepository.GetByIdAsync(id);
            if (existingAttendance == null)
            {
                logger.LogWarning("Attendance not found for update, ID: {Id}", id);
                return NotFound("Attendance not found.");
            }

            mapper.Map(attendanceDto, existingAttendance);
            unitOfWork.AttendanceRepository.Update(existingAttendance);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Successfully updated attendance ID: {Id}", id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            logger.LogInformation("Deleting attendance with ID: {Id}", id);
            if (id < 1)
            {
                logger.LogWarning("Invalid ID for deletion: {Id}", id);
                return BadRequest("Invalid data");
            }

            var attendance = await unitOfWork.AttendanceRepository.GetByIdAsync(id);
            if (attendance == null)
            {
                logger.LogWarning("Attendance not found for deletion: {Id}", id);
                return NotFound($"Attendance with ID {id} not found.");
            }

            unitOfWork.AttendanceRepository.Delete(attendance);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Deleted attendance with ID: {Id}", id);

            return NoContent();
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetAttendancesBySessionId(int sessionId)
        {
            logger.LogInformation("Fetching attendances for session ID: {SessionId}", sessionId);
            if (sessionId < 1)
                return BadRequest("Invalid data!");

            var res = await unitOfWork.AttendanceRepository.GetBySessionIdAsync(sessionId);
            if (res == null)
            {
                logger.LogWarning("No attendances found for session ID: {SessionId}", sessionId);
                return NotFound("No attendances found.");
            }

            var attendanceDto = mapper.Map<AttendanceDto>(res);
            return Ok(attendanceDto);
        }

        [HttpGet("ByDate")]
        public async Task<IActionResult> GetAttendancesByDate([FromQuery] DateTime date)
        {
            logger.LogInformation("Fetching attendances for date: {Date}", date);
            if (date > DateTime.Now)
                return BadRequest("Invalid data!");

            var res = await unitOfWork.AttendanceRepository.GetByDateAsync(date);
            if (res == null)
            {
                logger.LogWarning("No attendances found for date: {Date}", date);
                return NotFound("No attendances found.");
            }

            var attendanceDto = mapper.Map<IEnumerable<AttendanceDto>>(res);
            return Ok(attendanceDto);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetAttendanceByPatientIdAndDate(string patientId, [FromQuery] DateTime date)
        {
            logger.LogInformation("Fetching attendance for patient ID: {PatientId} on {Date}", patientId, date);
            if (patientId == null || date > DateTime.Now)
                return BadRequest("Invalid data!");

            var res = await unitOfWork.AttendanceRepository.GetByPatientIdAndDateAsync(patientId, date);
            if (res == null)
            {
                logger.LogWarning("No attendances found for patient ID: {PatientId} on {Date}", patientId, date);
                return NotFound("No attendances found.");
            }

            var attendanceDto = mapper.Map<AttendanceDto>(res);
            return Ok(attendanceDto);
        }

        [HttpGet("absent-patients")]
        public async Task<IActionResult> GetAbsentPatientsByDate([FromQuery] DateTime date)
        {
            logger.LogInformation("Fetching absent patients for date: {Date}", date);
            if (date > DateTime.Now)
                return BadRequest("Invalid date!");

            var absentPatients = await unitOfWork.AttendanceRepository.GetAbsentPatientsByDateAsync(date);
            if (absentPatients == null || !absentPatients.Any())
            {
                logger.LogWarning("No absent patients found for date: {Date}", date);
                return NotFound("No absent patients found for the specified date.");
            }

            var absentPatientDtos = mapper.Map<IEnumerable<PatientDto>>(absentPatients);
            return Ok(absentPatientDtos);
        }

        [HttpGet("daily-summary")]
        public async Task<IActionResult> GetDailySummaryReport([FromQuery] DateTime date)
        {
            logger.LogInformation("Generating daily summary report for date: {Date}", date);
            if (date > DateTime.Now)
                return BadRequest("Invalid date!");

            var summary = await unitOfWork.AttendanceRepository.GetDailySummaryReportAsync(date);
            if (summary == null)
            {
                logger.LogWarning("No attendance records found for summary on date: {Date}", date);
                return NotFound("No attendance records found for the specified date.");
            }

            return Ok(summary);
        }
    }
}
