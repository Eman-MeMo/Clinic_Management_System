using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.WorkScheduleDTOs;
using ClinicManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.WorkSchedules.UpdateWorkSchedule
{
    public class UpdateWorkScheduleHandler:IRequestHandler<UpdateWorkScheduleCommand, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        public UpdateWorkScheduleHandler(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public async Task<Unit> Handle(UpdateWorkScheduleCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null");
            }
            if (request.EndTime <= request.StartTime)
            {
                throw new ArgumentException("End time must be later than start time.");
            }
            
            var existingWorkSchedule = await unitOfWork.WorkScheduleRepository.GetByIdAsync(request.Id);
            if (existingWorkSchedule == null)
            {
                throw new KeyNotFoundException($"Work schedule with ID {request.Id} not found.");
            }

            existingWorkSchedule.StartTime = request.StartTime;
            existingWorkSchedule.EndTime = request.EndTime;
            existingWorkSchedule.DayOfWeek = request.DayOfWeek;
            existingWorkSchedule.IsAvailable = request.IsAvailable;
            existingWorkSchedule.DoctorId = request.DoctorId;

            unitOfWork.WorkScheduleRepository.Update(existingWorkSchedule);
            await unitOfWork.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
