using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Services
{
    public class DoctorAvailabilityService:IDoctorAvailabilityService
    {
        private readonly IUnitOfWork unitOfWork;
        
        public DoctorAvailabilityService(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public async Task<bool> IsDoctorAvailableAsync(string doctorId, DateTime date, int? appointmentIdToExclude = null)
        {
            // 1️ Check Work Schedule
            var schedules = await unitOfWork.WorkScheduleRepository.GetScheduleByDoctorAndDayAsync(doctorId, date.DayOfWeek);

            var workSchedule = schedules.FirstOrDefault(ws =>
            ws.IsAvailable &&
            date >= ws.StartTime &&
            date <= ws.EndTime);


            if (workSchedule == null)
                return false; // Doctor is not available on this day or time

            // 2️ Check Appointment conflicts
            var hasConflict = await unitOfWork.AppointmentRepository.HasAppointmentForDoctorAtDateAsync(doctorId, date, appointmentIdToExclude);

            return !hasConflict;
        }
        public async Task<IEnumerable<Doctor>> GetAvailableDoctorsAtAsync(DateTime targetTime)
        {
            return await unitOfWork.DoctorRepository
                .GetAllAsQueryable().Where(d => d.IsAvaible)
                .Where(d => d.WorkSchedules.Any(ws =>
                    ws.IsAvailable &&
                    ws.DayOfWeek == targetTime.DayOfWeek &&
                    TimeOnly.FromDateTime(targetTime) >= TimeOnly.FromDateTime(ws.StartTime) &&
                    TimeOnly.FromDateTime(targetTime) <= TimeOnly.FromDateTime(ws.EndTime)
                ))
                .Where(d => !d.Appointments.Any(a =>
                    a.Date == targetTime
                ))
                .ToListAsync();
        }
    }
}
