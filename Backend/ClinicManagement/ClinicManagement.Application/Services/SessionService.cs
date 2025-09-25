using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Services
{
    public class SessionService
    {
        private readonly IUnitOfWork unitOfWork;
        public SessionService(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public async Task EndSession(int sessionId, SessionStatus status)
        {
            var session = await unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if (session != null)
            {
                session.Status = status;
                session.ActualEndTime = DateTime.Now;

                // Update related appointment
                if (session.Appointment != null)
                {
                    session.Appointment.Status = status == SessionStatus.Done
                                                 ? AppointmentStatus.Confirmed
                                                 : AppointmentStatus.Cancelled;
                }

                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}
