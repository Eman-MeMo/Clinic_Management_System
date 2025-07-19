using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.WorkScheduleDTOs;
using ClinicManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkScheduleController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IAuditLoggerService auditLogger;
        private string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public WorkScheduleController(IUnitOfWork _unitOfWork, IMapper _mapper, IAuditLoggerService _auditLogger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            auditLogger = _auditLogger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllWorkSchedule()
        {
            var workSchedules = await unitOfWork.WorkScheduleRepository.GetAllAsync();
            if (workSchedules == null || !workSchedules.Any())
            {
                return NotFound("No work schedules found.");
            }
            var workScheduleDtos = mapper.Map<IEnumerable<WorkScheduleDto>>(workSchedules);
            return Ok(workScheduleDtos);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkScheduleById(int id)
        {
            var workSchedule = await unitOfWork.WorkScheduleRepository.GetByIdAsync(id);
            if (workSchedule == null)
            {
                return NotFound($"Work schedule with ID {id} not found.");
            }
            var workScheduleDto = mapper.Map<WorkScheduleDto>(workSchedule);
            return Ok(workScheduleDto);
        }
        [HttpPost]
        public async Task<IActionResult> CreateWorkSchedule([FromBody] CreateWorkScheduleDto workScheduleDto)
        {
            if (workScheduleDto == null)
            {
                return BadRequest("Work schedule data is null.");
            }
            if (workScheduleDto.EndTime <= workScheduleDto.StartTime)
            {
                return BadRequest("End time must be after start time.");
            }
            var workSchedule = mapper.Map<WorkSchedule>(workScheduleDto);
            await unitOfWork.WorkScheduleRepository.AddAsync(workSchedule);
            await unitOfWork.SaveChangesAsync();

            // Log the creation of the work schedule
            await auditLogger.LogAsync(
                action: "Create",
                tableName: nameof(WorkSchedule),
                details: $"Created work schedule for doctor {workSchedule.DoctorId} on {workSchedule.DayOfWeek}",
                userId: UserId
            );
            return CreatedAtAction(nameof(GetWorkScheduleById), new { id = workSchedule.Id }, workScheduleDto);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkSchedule(int id, [FromBody] WorkScheduleDto workScheduleDto)
        {
            if (workScheduleDto == null || id != workScheduleDto.Id)
            {
                return BadRequest("Work schedule data is invalid.");
            }
            if (workScheduleDto.EndTime <= workScheduleDto.StartTime)
            {
                return BadRequest("End time must be after start time.");
            }
            var existingWorkSchedule = await unitOfWork.WorkScheduleRepository.GetByIdAsync(id);
            if (existingWorkSchedule == null)
            {
                return NotFound($"Work schedule with ID {id} not found.");
            }
            var workSchedule = mapper.Map<WorkSchedule>(workScheduleDto);
            unitOfWork.WorkScheduleRepository.Update(workSchedule);
            await unitOfWork.SaveChangesAsync();

            // Log the update of the work schedule
            await auditLogger.LogAsync(
                action: "Update",
                tableName: nameof(WorkSchedule),
                details: $"Updated work schedule for doctor {workSchedule.DoctorId} on {workSchedule.DayOfWeek}",
                userId: UserId
            );
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkSchedule(int id)
        {
            var workSchedule = await unitOfWork.WorkScheduleRepository.GetByIdAsync(id);
            if (workSchedule == null)
            {
                return NotFound($"Work schedule with ID {id} not found.");
            }
            unitOfWork.WorkScheduleRepository.Delete(workSchedule);
            await unitOfWork.SaveChangesAsync();

            // Log the deletion of the work schedule
            await auditLogger.LogAsync(
                action: "Delete",
                tableName: nameof(WorkSchedule),
                details: $"Deleted work schedule for doctor {workSchedule.DoctorId} on {workSchedule.DayOfWeek}",
                userId: UserId
            );
            return NoContent();
        }
        [HttpGet("doctor/{doctorId}/day/{dayOfWeek}")]
        public async Task<IActionResult> GetScheduleByDoctorAndDay(string doctorId, DayOfWeek dayOfWeek)
        {
            if (string.IsNullOrEmpty(doctorId))
            {
                return BadRequest("Doctor ID cannot be null or empty.");
            }
            if (!Enum.IsDefined(typeof(DayOfWeek), dayOfWeek))
            {
                return BadRequest("Invalid day of the week.");
            }
            var workSchedules = await unitOfWork.WorkScheduleRepository.GetScheduleByDoctorAndDayAsync(doctorId, dayOfWeek);
            if (workSchedules == null || !workSchedules.Any())
            {
                return NotFound($"No work schedules found for doctor {doctorId} on {dayOfWeek}.");
            }
            var workScheduleDtos = mapper.Map<IEnumerable<WorkScheduleDto>>(workSchedules);
            return Ok(workScheduleDtos);
        }
        [HttpGet("doctor/{doctorId}/weekly")]
        public async Task<IActionResult> GetWeeklySchedule(string doctorId)
        {
            if (string.IsNullOrEmpty(doctorId))
            {
                return BadRequest("Doctor ID cannot be null or empty.");
            }
            var workSchedules = await unitOfWork.WorkScheduleRepository.GetWeeklyScheduleAsync(doctorId);
            if (workSchedules == null || !workSchedules.Any())
            {
                return NotFound($"No weekly schedule found for doctor {doctorId}.");
            }
            var workScheduleDtos = mapper.Map<IEnumerable<WorkScheduleDto>>(workSchedules);
            return Ok(workScheduleDtos);
        }
        [HttpGet("availability")]
        public async Task<IActionResult> CheckDoctorAvailability(string doctorId, DateTime dateTime)
        {
            if (string.IsNullOrEmpty(doctorId))
            {
                return BadRequest("Doctor ID cannot be null or empty.");
            }
            if (dateTime == default)
            {
                return BadRequest("Invalid date and time.");
            }
            var isAvailable = await unitOfWork.WorkScheduleRepository.CheckAvailabilityAsync(doctorId, dateTime);
            return Ok(new { IsAvailable = isAvailable });
        }
    }
}
