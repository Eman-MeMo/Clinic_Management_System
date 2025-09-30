using AutoMapper;
using ClinicManagement.Application.Commands.Appointments.BookAppointment;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Appointments.CancelAppointment
{
    public class CancelAppointmentHandler : IRequestHandler<CancelAppointmentCommand, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public CancelAppointmentHandler(IUnitOfWork _unitOfWork, IMapper mapper)
        {
            unitOfWork = _unitOfWork;
            this.mapper = mapper;
        }
        public async Task<Unit> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
        {
            if (request == null) 
                throw new ArgumentNullException(nameof(request));
            
            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(request.Id);

            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {request.Id} not found.");
            
            bool hasSession = await unitOfWork.SessionRepository.HasSessionForAppointmentAsync(request.Id);

            if (hasSession)
                throw new InvalidOperationException("Cannot cancel appointment. A session has already been started for this appointment.");
            
            appointment.Status = AppointmentStatus.Cancelled;
            unitOfWork.AppointmentRepository.Update(appointment);
            await unitOfWork.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
