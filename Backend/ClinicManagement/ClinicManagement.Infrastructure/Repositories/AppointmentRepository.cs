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
        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, AppointmentStatus status)
        {
            var appointment = await db.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);
            if (appointment == null) return false;

            appointment.Status = status;
            await db.SaveChangesAsync();
            return true;
        }
    }
}
