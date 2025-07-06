using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Infrastructure.Repositories
{
    public class AppointmentRepository:GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ClinicDbContext db):base(db)
        {
            
        }
        public async Task<IEnumerable<Appointment>> GetAllByDoctorIdAsync(string doctorId)
        {
            return await db.Appointments.Where(p=>p.DoctorId==doctorId).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAllByPatientIdAsync(string pateintId)
        {
            return await db.Appointments.Where(p => p.PatientId == pateintId).ToListAsync();
        }
        public async Task<bool> IsDoctorAvailableAsync(string doctorId, DateTime date, int? appointmentIdToExclude = null)
        {
            // Check if the doctor exists
            var workSchedule = await db.WorkSchedules
                .FirstOrDefaultAsync(ws =>
                    ws.DoctorId == doctorId &&
                    ws.IsAvailable == true &&
                    ws.DayOfWeek == date.DayOfWeek &&
                    TimeOnly.FromDateTime(date) >= TimeOnly.FromDateTime(ws.StartTime) &&
                    TimeOnly.FromDateTime(date) <= TimeOnly.FromDateTime(ws.EndTime)
                );

            if (workSchedule == null)
            {
                return false; // Doctor is not available on this day or time
            }

            // Check for appointment conflicts
            var hasConflict = await db.Appointments
                .AnyAsync(a =>
                    a.DoctorId == doctorId &&
                    a.Date == date &&
                    (appointmentIdToExclude == null || a.Id != appointmentIdToExclude) // Exclude the current appointment if provided
                );

            return !hasConflict;
        }

    }
}
