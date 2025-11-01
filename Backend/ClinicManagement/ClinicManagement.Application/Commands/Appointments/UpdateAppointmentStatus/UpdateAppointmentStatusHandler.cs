using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Appointments.UpdateAppointmentStatus
{
    public class UpdateAppointmentStatusHandler : IRequestHandler<UpdateAppointmentStatusCommand, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        public UpdateAppointmentStatusHandler(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public async Task<Unit> Handle(UpdateAppointmentStatusCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(request.AppointmentId);
            if (appointment == null)
            {
                throw new KeyNotFoundException($"Appointment with ID {request.AppointmentId} not found.");
            }
            appointment.Status = request.Status;
            unitOfWork.AppointmentRepository.Update(appointment);
            await unitOfWork.SaveChangesAsync();
            return Unit.Value;
        }
    }
}
