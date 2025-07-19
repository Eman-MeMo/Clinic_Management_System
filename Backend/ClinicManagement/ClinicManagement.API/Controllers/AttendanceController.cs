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
        private readonly IAuditLoggerService auditLogger;
        private string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public AttendanceController(IUnitOfWork _unitOfWork, IMapper _mapper, IAuditLoggerService _auditLogger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            auditLogger = _auditLogger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAttendance([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var attendances = unitOfWork.AttendanceRepository.GetAllAsQueryable();
            var totalCount = await attendances.CountAsync();
            var paginationSkip = (pageNumber - 1) * pageSize;
            var items = await attendances
                .Skip(paginationSkip)
                .Take(pageSize)
                .ToListAsync();

            var attendanceDtos = mapper.Map<List<AttendanceDto>>(items);
            var result = new PaginatedResultDto<AttendanceDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = attendanceDtos
            };
            return Ok(result);

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttendanceById(int id)
        {
            if (id < 1)
                return BadRequest("Attendance ID cannot be null or empty.");

            var attendance = await unitOfWork.AttendanceRepository.GetByIdAsync(id);
            if (attendance == null)
                return NotFound("Attendance not found.");

            var attendanceDto = mapper.Map<AttendanceDto>(attendance);
            return Ok(attendanceDto);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAttendance([FromBody] CreateAppointmentDto attendanceDto)
        {
            var attendance = mapper.Map<Attendance>(attendanceDto);
            await unitOfWork.AttendanceRepository.AddAsync(attendance);
            await unitOfWork.SaveChangesAsync();

            // Log the creation of attendance
            await auditLogger.LogAsync("CreateAttendance", attendance.Id.ToString(), $"Attendance created for session {attendance.SessionId}.", UserId);

            return CreatedAtAction(nameof(GetAttendanceById), new { id = attendance.Id }, mapper.Map<AttendanceDto>(attendance));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttendance(int id, [FromBody] AttendanceDto attendanceDto)
        {
            if (id != attendanceDto.Id)
                return BadRequest("Invalid data");

            var existingAttendance = await unitOfWork.AttendanceRepository.GetByIdAsync(id);
            if (existingAttendance == null)
                return NotFound("Attendance not found.");

            mapper.Map(attendanceDto, existingAttendance);
            unitOfWork.AttendanceRepository.Update(existingAttendance);
            await unitOfWork.SaveChangesAsync();

            // Log the update of attendance
            await auditLogger.LogAsync("UpdateAttendance", existingAttendance.Id.ToString(), $"Attendance updated for session {existingAttendance.SessionId}.", UserId);
            
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            if (id < 1)
                return BadRequest("Invalid data");

            var attendance = await unitOfWork.AttendanceRepository.GetByIdAsync(id);
            if(attendance == null)
                return NotFound($"Attendance with ID {id} not found.");

            unitOfWork.AttendanceRepository.Delete(attendance);
            await unitOfWork.SaveChangesAsync();

            //Log the action
            await auditLogger.LogAsync("DeleteAttendance", attendance.Id.ToString(), $"Deleted attendance with ID {attendance.Id}", UserId);
            return NoContent();
        }
        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetAttendancesBySessionId(int sessionId)
        {
            if (sessionId < 1)
                return BadRequest("Invalid data!");

            var res=await unitOfWork.AttendanceRepository.GetBySessionIdAsync(sessionId);
            if (res == null)
                return NotFound("No attendances found.");

            var attendanceDto = mapper.Map<AttendanceDto>(res);
            return Ok(attendanceDto);
        }
        [HttpGet("ByDate")]
        public async Task<IActionResult> GetAttendancesByDate([FromQuery] DateTime date)
        {
            if (date > DateTime.Now)
            {
                return BadRequest("Invalid data!");
            }
            var res=await unitOfWork.AttendanceRepository.GetByDateAsync(date);
            if (res == null)
                return NotFound("No attendances found.");

            var attendanceDto = mapper.Map<IEnumerable<AttendanceDto>>(res);
            return Ok(attendanceDto);
        }
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetAttendanceByPatientIdAndDate(string patientId, [FromQuery] DateTime date)
        {
            if (patientId == null || date > DateTime.Now)
            {
                return BadRequest("Invalid data!");
            }
            var res = await unitOfWork.AttendanceRepository.GetByPatientIdAndDateAsync(patientId, date);
            if (res == null)
                return NotFound("No attendances found.");

            var attendanceDto = mapper.Map<AttendanceDto>(res);
            return Ok(attendanceDto);
        }
        [HttpGet("absent-patients")]
        public async Task<IActionResult> GetAbsentPatientsByDate([FromQuery] DateTime date)
        {
            if (date > DateTime.Now)
            {
                return BadRequest("Invalid date!");
            }
            var absentPatients = await unitOfWork.AttendanceRepository.GetAbsentPatientsByDateAsync(date);
            if (absentPatients == null || !absentPatients.Any())
            {
                return NotFound("No absent patients found for the specified date.");
            }
            var absentPatientDtos = mapper.Map<IEnumerable<PatientDto>>(absentPatients);
            return Ok(absentPatientDtos);
        }
        [HttpGet("daily-summary")]
        public async Task<IActionResult> GetDailySummaryReport([FromQuery] DateTime date)
        {
            if (date > DateTime.Now)
            {
                return BadRequest("Invalid date!");
            }
            var summary = await unitOfWork.AttendanceRepository.GetDailySummaryReportAsync(date);
            if (summary == null)
            {
                return NotFound("No attendance records found for the specified date.");
            }
            return Ok(summary);
        }
    }
}
