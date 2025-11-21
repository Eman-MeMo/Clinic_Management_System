using AutoMapper;
using ClinicManagement.Application.Commands.Attendances.MarkAbsent;
using ClinicManagement.Application.Commands.Attendances.MarkPresent;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.Attendances.GetDailyAttendanceSummary;
using ClinicManagement.Application.Queries.Attendances.GetPatientAttendanceHistory;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using ClinicManagement.Domain.DTOs.AttendanceDTOs;
using ClinicManagement.Domain.DTOs.Pagination;
using ClinicManagement.Domain.DTOs.PatientDTOs;
using ClinicManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Doctor")]
    public class AttendanceController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public AttendanceController(IUnitOfWork _unitOfWork, IMapper _mapper,IMediator _mediator)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            mediator = _mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAttendance([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
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
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttendanceById(int id)
        {
            if (id < 1)
            {
                return BadRequest("Attendance ID cannot be null or empty.");
            }

            var attendance = await unitOfWork.AttendanceRepository.GetByIdAsync(id);
            if (attendance == null)
            {
                return NotFound("Attendance not found.");
            }

            var attendanceDto = mapper.Map<AttendanceDto>(attendance);
            return Ok(attendanceDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAttendance([FromBody] CreateAttendaceDto attendanceDto)
        {
            var attendance = mapper.Map<Attendance>(attendanceDto);
            var patientSessions = await unitOfWork.SessionRepository.GetSessionsByPatient(attendanceDto.PatientId); 
            if (!patientSessions.Any(s => s.Id == attendanceDto.SessionId)) 
            {
                return BadRequest("The session does not belong to this patient or does not exist.");
            }
            await unitOfWork.AttendanceRepository.AddAsync(attendance);
            await unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAttendanceById), new { id = attendance.Id }, attendance);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttendance(int id, [FromBody] AttendanceDto attendanceDto)
        {
            if (id != attendanceDto.Id)
            {
                return BadRequest("Invalid data");
            }

            var existingAttendance = await unitOfWork.AttendanceRepository.GetByIdAsync(id);
            if (existingAttendance == null)
            {
                return NotFound("Attendance not found.");
            }

            mapper.Map(attendanceDto, existingAttendance);
            unitOfWork.AttendanceRepository.Update(existingAttendance);
            await unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            if (id < 1)
            {
                return BadRequest("Invalid data");
            }

            var attendance = await unitOfWork.AttendanceRepository.GetByIdAsync(id);
            if (attendance == null)
            {
                return NotFound($"Attendance with ID {id} not found.");
            }

            unitOfWork.AttendanceRepository.Delete(attendance);
            await unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetAttendancesBySessionId(int sessionId)
        {
            if (sessionId < 1)
                return BadRequest("Invalid data!");

            var res = await unitOfWork.AttendanceRepository.GetBySessionIdAsync(sessionId);
            if (res == null)
            {
                return NotFound("No attendances found.");
            }

            var attendanceDto = mapper.Map<AttendanceDto>(res);
            return Ok(attendanceDto);
        }

        [HttpGet("ByDate")]
        public async Task<IActionResult> GetAttendancesByDate([FromQuery] DateTime date)
        {
            if (date > DateTime.Now)
                return BadRequest("Invalid data!");

            var res = await unitOfWork.AttendanceRepository.GetByDateAsync(date);
            if (res == null)
            {
                return NotFound("No attendances found.");
            }

            var attendanceDto = mapper.Map<IEnumerable<AttendanceDto>>(res);
            return Ok(attendanceDto);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetAttendanceByPatientIdAndDate(string patientId, [FromQuery] DateTime date)
        {
            if (patientId == null || date > DateTime.Now)
                return BadRequest("Invalid data!");

            var res = await unitOfWork.AttendanceRepository.GetByPatientIdAndDateAsync(patientId, date);
            if (res == null)
            {
                return NotFound("No attendances found.");
            }

            var attendanceDto = mapper.Map<AttendanceDto>(res);
            return Ok(attendanceDto);
        }

        [HttpGet("absent-patients/{date}")]
        public async Task<IActionResult> GetAbsentPatientsByDate(DateTime date)
        {
            if (date > DateTime.Now)
                return BadRequest("Invalid date!");

            var absentPatients = await unitOfWork.AttendanceRepository.GetAbsentPatientsByDateAsync(date);
            if (absentPatients == null || !absentPatients.Any())
            {
                return NotFound("No absent patients found for the specified date.");
            }

            var absentPatientDtos = mapper.Map<IEnumerable<PatientDto>>(absentPatients);
            return Ok(absentPatientDtos);
        }
        [HttpPut("mark-absent")]
        public async Task<IActionResult> MarkAttendanceAsAbsent([FromBody] MarkAbsentCommand command)
        {
            await mediator.Send(command);
            return Ok("Attendance Status Updated to Absent");
        }
        [HttpPut("mark-present")]
        public async Task<IActionResult> MarkAttendanceAsPresent([FromBody] MarkPresentCommand command)
        {
            await mediator.Send(command);
            return Ok("Attendance Status Updated to Present");
        }
        [HttpGet("daily-summary")]
        public async Task<IActionResult> GetDailySummaryReport([FromQuery] DateTime date)
        {
            if (date > DateTime.Now)
                return BadRequest("Invalid date!");

            var summary = await mediator.Send(new GetDailyAttendanceSummaryQuery { Date = date });
            if (summary == null)
            {
                return NotFound("No attendance records found for the specified date.");
            }
            return Ok(summary);
        }
        [HttpGet("PatientAttendanceHistory")]
        public async Task<IActionResult> GetPatientAttendanceHistory([FromQuery] string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
            {
                return BadRequest("Patient ID cannot be null or empty.");
            }

            var query = new GetPatientAttendanceHistoryQuery { PatientId = patientId };
            var history = await mediator.Send(query);

            if (history == null || !history.Any())
            {
                return NotFound("No attendance history found.");
            }
            return Ok(history);
        }

    }
}
