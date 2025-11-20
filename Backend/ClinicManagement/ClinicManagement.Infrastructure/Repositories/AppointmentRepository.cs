using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.DTOs.AppointmentDTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClinicManagement.Infrastructure.Repositories
{
    public class AppointmentRepository:GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ClinicDbContext db):base(db)
        {
            
        }
        public async Task<IEnumerable<Appointment>> GetAllByDoctorIdAsync(string doctorId)
        {
            return await db.Appointments.AsNoTracking().Where(p=>p.DoctorId==doctorId).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAllByPatientIdAsync(string pateintId)
        {
            return await db.Appointments.AsNoTracking().Where(p => p.PatientId == pateintId).ToListAsync();
        }
        public async Task<bool> HasAppointmentForDoctorAtDateAsync(string doctorId, DateTime date, int? appointmentIdToExclude = null)
        {
            return await db.Appointments
            .AnyAsync(a =>
                    a.DoctorId == doctorId &&
                    a.Date == date &&
                    (appointmentIdToExclude == null || a.Id != appointmentIdToExclude)
                );
        }
        public async Task<bool> HasAppointmentForDoctorAsync(string doctorId)
        {
            return await db.Appointments
                .AnyAsync(a => a.DoctorId == doctorId && a.Date > DateTime.Now);
        }
        public async Task CancelAllAppointmentsForPatient(string patientId)
        {
            var appointments = await db.Appointments
                .Where(a => a.PatientId == patientId && a.Status == AppointmentStatus.Scheduled)
                .ToListAsync();
            foreach (var appointment in appointments)
            {
                appointment.Status = AppointmentStatus.Cancelled;
            }
            db.Appointments.UpdateRange(appointments);
        }
        public async Task<bool> CanPatientBook(string patientId, DateTime date)
        {
            // Check if patient already has an appointment at the same date/time
            var overlapping = await db.Appointments
                .Where(a => a.PatientId == patientId
                            && a.Date == date
                            && a.Status != AppointmentStatus.Cancelled)
                .AnyAsync();

            return !overlapping;
        }

    }
}
