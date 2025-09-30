using AutoMapper;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Appointments.UpdateAppointment
{
    public class UpdateAppointmentHandler:IRequestHandler<UpdateAppointmentCommand, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IDoctorAvailabilityService doctorAvailabilityService;
        public UpdateAppointmentHandler(IUnitOfWork _unitOfWork, IMapper mapper, IDoctorAvailabilityService _doctorAvailabilityService)
        {
            unitOfWork = _unitOfWork;
            this.mapper = mapper;
            doctorAvailabilityService = _doctorAvailabilityService;
        }
        public async Task<Unit> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var existingAppointment = await unitOfWork.AppointmentRepository.GetByIdAsync(request.Id);
            if (existingAppointment == null)
                throw new KeyNotFoundException($"Appointment with ID {request.Id} not found.");

            var isDoctorAvailable = await doctorAvailabilityService.IsDoctorAvailableAsync(request.DoctorId, request.Date, request.Id);
            if (!isDoctorAvailable)
                throw new InvalidOperationException("The doctor is not available at the requested time.");

            mapper.Map(request, existingAppointment);
            unitOfWork.AppointmentRepository.Update(existingAppointment);
            await unitOfWork.SaveChangesAsync();
            return Unit.Value;

        }
    }
}
