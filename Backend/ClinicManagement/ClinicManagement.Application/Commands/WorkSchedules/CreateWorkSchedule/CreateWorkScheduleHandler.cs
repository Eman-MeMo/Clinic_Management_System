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

namespace ClinicManagement.Application.Commands.WorkSchedules.CreateWorkSchedule
{
    public class CreateWorkScheduleHandler:IRequestHandler<CreateWorkScheduleCommand, int>
    {
        private readonly IUnitOfWork unitOfWork;
        public CreateWorkScheduleHandler(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public async Task<int> Handle(CreateWorkScheduleCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null");
            }
            if (request.EndTime <= request.StartTime)
            {
                throw new ArgumentException("End time must be later than start time.");
            }
            var workSchedule = new WorkSchedule
            {
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                DayOfWeek = request.DayOfWeek,
                IsAvailable = request.IsAvailable,
                DoctorId = request.DoctorId
            };
            await unitOfWork.WorkScheduleRepository.AddAsync(workSchedule);
            await unitOfWork.SaveChangesAsync();
            return workSchedule.Id;
        }
    }
}
