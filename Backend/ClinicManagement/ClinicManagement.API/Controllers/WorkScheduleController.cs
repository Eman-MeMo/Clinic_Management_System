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
        private readonly ILogger<WorkScheduleController> logger;

        public WorkScheduleController(IUnitOfWork _unitOfWork, IMapper _mapper, ILogger<WorkScheduleController> _logger)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            logger = _logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWorkSchedule()
        {
            logger.LogInformation("Getting all work schedules...");
            var workSchedules = await unitOfWork.WorkScheduleRepository.GetAllAsync();
            if (workSchedules == null || !workSchedules.Any())
            {
                logger.LogWarning("No work schedules found.");
                return NotFound("No work schedules found.");
            }
            logger.LogInformation("Retrieved {Count} work schedules.", workSchedules.Count());
            var workScheduleDtos = mapper.Map<IEnumerable<WorkScheduleDto>>(workSchedules);
            return Ok(workScheduleDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkScheduleById(int id)
        {
            logger.LogInformation("Getting work schedule with ID: {Id}", id);
            var workSchedule = await unitOfWork.WorkScheduleRepository.GetByIdAsync(id);
            if (workSchedule == null)
            {
                logger.LogWarning("Work schedule with ID {Id} not found.", id);
                return NotFound($"Work schedule with ID {id} not found.");
            }
            logger.LogInformation("Work schedule with ID {Id} retrieved successfully.", id);
            var workScheduleDto = mapper.Map<WorkScheduleDto>(workSchedule);
            return Ok(workScheduleDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkSchedule([FromBody] CreateWorkScheduleDto workScheduleDto)
        {
            logger.LogInformation("Creating a new work schedule...");
            if (workScheduleDto == null)
            {
                logger.LogWarning("Received null work schedule data.");
                return BadRequest("Work schedule data is null.");
            }

            if (workScheduleDto.EndTime <= workScheduleDto.StartTime)
            {
                logger.LogWarning("Invalid time range: StartTime {Start} - EndTime {End}", workScheduleDto.StartTime, workScheduleDto.EndTime);
                return BadRequest("End time must be after start time.");
            }

            var workSchedule = mapper.Map<WorkSchedule>(workScheduleDto);
            await unitOfWork.WorkScheduleRepository.AddAsync(workSchedule);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Work schedule created successfully with ID {Id}", workSchedule.Id);

            return CreatedAtAction(nameof(GetWorkScheduleById), new { id = workSchedule.Id }, workScheduleDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkSchedule(int id, [FromBody] WorkScheduleDto workScheduleDto)
        {
            logger.LogInformation("Updating work schedule with ID {Id}", id);

            if (workScheduleDto == null || id != workScheduleDto.Id)
            {
                logger.LogWarning("Invalid update request. ID mismatch or null DTO.");
                return BadRequest("Work schedule data is invalid.");
            }

            if (workScheduleDto.EndTime <= workScheduleDto.StartTime)
            {
                logger.LogWarning("Invalid time range during update: StartTime {Start} - EndTime {End}", workScheduleDto.StartTime, workScheduleDto.EndTime);
                return BadRequest("End time must be after start time.");
            }

            var existingWorkSchedule = await unitOfWork.WorkScheduleRepository.GetByIdAsync(id);
            if (existingWorkSchedule == null)
            {
                logger.LogWarning("Work schedule with ID {Id} not found for update.", id);
                return NotFound($"Work schedule with ID {id} not found.");
            }

            var workSchedule = mapper.Map<WorkSchedule>(workScheduleDto);
            unitOfWork.WorkScheduleRepository.Update(workSchedule);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Work schedule with ID {Id} updated successfully.", id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkSchedule(int id)
        {
            logger.LogInformation("Deleting work schedule with ID {Id}", id);
            var workSchedule = await unitOfWork.WorkScheduleRepository.GetByIdAsync(id);
            if (workSchedule == null)
            {
                logger.LogWarning("Work schedule with ID {Id} not found for deletion.", id);
                return NotFound($"Work schedule with ID {id} not found.");
            }

            unitOfWork.WorkScheduleRepository.Delete(workSchedule);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Work schedule with ID {Id} deleted successfully.", id);

            return NoContent();
        }

        [HttpGet("doctor/{doctorId}/day/{dayOfWeek}")]
        public async Task<IActionResult> GetScheduleByDoctorAndDay(string doctorId, DayOfWeek dayOfWeek)
        {
            logger.LogInformation("Retrieving schedule for doctor {DoctorId} on {Day}", doctorId, dayOfWeek);

            if (string.IsNullOrEmpty(doctorId))
            {
                logger.LogWarning("Doctor ID is null or empty.");
                return BadRequest("Doctor ID cannot be null or empty.");
            }

            if (!Enum.IsDefined(typeof(DayOfWeek), dayOfWeek))
            {
                logger.LogWarning("Invalid day of the week: {Day}", dayOfWeek);
                return BadRequest("Invalid day of the week.");
            }

            var workSchedules = await unitOfWork.WorkScheduleRepository.GetScheduleByDoctorAndDayAsync(doctorId, dayOfWeek);
            if (workSchedules == null || !workSchedules.Any())
            {
                logger.LogWarning("No work schedules found for doctor {DoctorId} on {Day}", doctorId, dayOfWeek);
                return NotFound($"No work schedules found for doctor {doctorId} on {dayOfWeek}.");
            }

            logger.LogInformation("Found {Count} work schedules for doctor {DoctorId} on {Day}", workSchedules.Count(), doctorId, dayOfWeek);

            var workScheduleDtos = mapper.Map<IEnumerable<WorkScheduleDto>>(workSchedules);
            return Ok(workScheduleDtos);
        }

        [HttpGet("doctor/{doctorId}/weekly")]
        public async Task<IActionResult> GetWeeklySchedule(string doctorId)
        {
            logger.LogInformation("Retrieving weekly schedule for doctor {DoctorId}", doctorId);

            if (string.IsNullOrEmpty(doctorId))
            {
                logger.LogWarning("Doctor ID is null or empty.");
                return BadRequest("Doctor ID cannot be null or empty.");
            }

            var workSchedules = await unitOfWork.WorkScheduleRepository.GetWeeklyScheduleAsync(doctorId);
            if (workSchedules == null || !workSchedules.Any())
            {
                logger.LogWarning("No weekly schedule found for doctor {DoctorId}.", doctorId);
                return NotFound($"No weekly schedule found for doctor {doctorId}.");
            }

            logger.LogInformation("Weekly schedule for doctor {DoctorId} retrieved successfully.", doctorId);

            var workScheduleDtos = mapper.Map<IEnumerable<WorkScheduleDto>>(workSchedules);
            return Ok(workScheduleDtos);
        }

        [HttpGet("availability")]
        public async Task<IActionResult> CheckDoctorAvailability(string doctorId, DateTime dateTime)
        {
            logger.LogInformation("Checking availability for doctor {DoctorId} at {DateTime}", doctorId, dateTime);

            if (string.IsNullOrEmpty(doctorId))
            {
                logger.LogWarning("Doctor ID is null or empty.");
                return BadRequest("Doctor ID cannot be null or empty.");
            }

            if (dateTime == default)
            {
                logger.LogWarning("Invalid date and time provided.");
                return BadRequest("Invalid date and time.");
            }

            var isAvailable = await unitOfWork.WorkScheduleRepository.CheckAvailabilityAsync(doctorId, dateTime);

            logger.LogInformation("Doctor {DoctorId} availability at {DateTime}: {IsAvailable}", doctorId, dateTime, isAvailable);

            return Ok(new { IsAvailable = isAvailable });
        }
    }
}
