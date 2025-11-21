using AutoMapper;
using ClinicManagement.Application.Commands.WorkSchedules.CreateWorkSchedule;
using ClinicManagement.Application.Commands.WorkSchedules.UpdateWorkSchedule;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Queries.WorkSchedules.CheckDoctorAvailability;
using ClinicManagement.Application.Queries.WorkSchedules.GetScheduleByDoctorAndDay;
using ClinicManagement.Application.Queries.WorkSchedules.GetWeeklySchedule;
using ClinicManagement.Domain.DTOs.WorkScheduleDTOs;
using ClinicManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Doctor")]
    public class WorkScheduleController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public WorkScheduleController(IUnitOfWork _unitOfWork, IMapper _mapper, IMediator _mediator)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            mediator = _mediator;
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
        public async Task<IActionResult> CreateWorkSchedule([FromBody] CreateWorkScheduleCommand command)
        {      
            if(command==null)
                return BadRequest("Work schedule data is invalid.");

            var workScheduleId=await mediator.Send(command);
            return CreatedAtAction(nameof(GetWorkScheduleById), new { id = workScheduleId },command);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkSchedule(int id, UpdateWorkScheduleCommand command)
        {
            if (command == null || id != command.Id)
            {
                return BadRequest("Work schedule data is invalid.");
            }

            await mediator.Send(command);
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

            var workSchedules = await mediator.Send(new GetScheduleByDoctorAndDayQuery { DoctorId = doctorId, Day = dayOfWeek });
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

            var workSchedules = await mediator.Send(new GetWeeklyScheduleQuery { DoctorId = doctorId });
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

            var isAvailable = await mediator.Send(new CheckDoctorAvailabilityQuery
            {
                DoctorId = doctorId,
                AppointmentDateTime = dateTime
            });
            return Ok(new { IsAvailable = isAvailable });
        }
    }
}
