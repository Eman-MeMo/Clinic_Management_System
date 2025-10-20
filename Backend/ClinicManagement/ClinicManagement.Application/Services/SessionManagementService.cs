using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Services
{
    public class SessionManagementService:ISessionService
    {
        private readonly IUnitOfWork unitOfWork;
        public SessionManagementService(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public async Task EndSessionAsync(int sessionId, SessionStatus status)
        {
            var session = await unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if (session == null)
                throw new InvalidOperationException("Session not found.");

            if (session.Status == SessionStatus.Scheduled &&
                (status == SessionStatus.Confirmed || status == SessionStatus.Cancelled))
            {
                session.Status = status;
                session.ActualEndTime = DateTime.UtcNow;

                // Update related appointment
                if (session.Appointment != null)
                {
                    session.Appointment.Status = status == SessionStatus.Confirmed
                                                 ? AppointmentStatus.Confirmed
                                                 : AppointmentStatus.Cancelled;

                    unitOfWork.AppointmentRepository.Update(session.Appointment);
                }

                unitOfWork.SessionRepository.Update(session);
                await unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Only scheduled sessions can be ended with status 'Done' or 'Cancelled'.");
            }
        }
        public async Task<int> StartSessionAsync(int appointmentId)
        {
            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null)
                throw new InvalidOperationException("Appointment not found.");

            var result = await unitOfWork.SessionRepository.HasSessionForAppointmentAsync(appointmentId);
            if (result)
                throw new InvalidOperationException($"A session already exists for appointment ID {appointmentId}.");

            var allowedStartTime = appointment.Date.AddMinutes(-10);
            if (DateTime.Now < allowedStartTime)
                throw new InvalidOperationException("Cannot start session before the scheduled time.");

            if (appointment.Status != AppointmentStatus.Confirmed)
                throw new InvalidOperationException("Appointment must be confirmed before starting a session.");

            return await unitOfWork.SessionRepository.CreateSessionAsync(appointmentId);
        }
    }
}